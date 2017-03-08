/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { AutoRestConfigurationManager, AutoRestConfiguration } from "../configuration/configuration";
import { DataStoreView, DataHandleRead, DataStoreViewReadonly, KnownScopes } from "../data-store/data-store";
import { parse } from "../parsing/literateYaml";
import { mergeYamls, identitySourceMapping } from "../source-map/merging";
import { MultiPromiseUtility, MultiPromise } from "../approved-imports/multi-promise";
import { CancellationToken } from "../approved-imports/cancallation";
import { AutoRestPlugin } from "./plugin-server";
import { JsonPath, JsonPathComponent } from "../approved-imports/jsonpath";
import { resolveRelativeNode } from "../parsing/yaml";
import { descendants, YAMLNodeWithPath, toAst } from "../approved-imports/yaml";
import { resolveUri } from "../approved-imports/uri";
import { From } from "../approved-imports/linq";
import { Mapping } from "../approved-imports/source-map";
import { CreateAssignmentMapping } from "../source-map/source-map";

export type DataPromise = MultiPromise<DataHandleRead>;

async function LoadUri(inputScope: DataStoreViewReadonly, inputFileUri: string): Promise<DataHandleRead> {
  const handle = await inputScope.Read(inputFileUri);
  if (handle === null) {
    throw new Error(`Input file '${inputFileUri}' not found.`);
  }
  return handle;
}

async function DeliteralizeYaml(literate: DataHandleRead, workingScope: DataStoreView): Promise<DataHandleRead> {
  const docScope = workingScope.CreateScope(`doc_tmp`);
  const hwRawDoc = await workingScope.Write(`doc.yaml`);
  const hRawDoc = await parse(literate, hwRawDoc, docScope);
  return hRawDoc;
}

async function LoadLiterateYaml(inputScope: DataStoreViewReadonly, inputFileUri: string, workingScope: DataStoreView): Promise<DataHandleRead> {
  const pluginSwaggerInput = await LoadUri(inputScope, inputFileUri);
  const pluginDeliteralizeSwagger = DeliteralizeYaml(pluginSwaggerInput, workingScope);
  return pluginDeliteralizeSwagger;
}

async function EnsureCompleteDefinitionIsPresent(
  inputScope: DataStoreViewReadonly,
  workingScope: DataStoreView,
  visitedEntities: string[],
  externalFiles: { [uri: string]: DataHandleRead },
  sourceFileUri: string,
  currentFileUri?: string,
  entityType?: string,
  modelName?: string) {

  const references: YAMLNodeWithPath[] = [];
  const sourceDoc = externalFiles[sourceFileUri];
  if (currentFileUri == null) {
    currentFileUri = sourceFileUri;
  }

  var currentDoc = await externalFiles[currentFileUri].ReadYamlAst();
  if (entityType == null || modelName == null) {
    // external references
    for (const node of descendants(currentDoc)) {
      if (node.path[node.path.length - 1] === "$ref") {
        if (!(node.node.value as string).startsWith("#")) {
          references.push(node);
        }
      }
    }
  } else {
    // references within external file
    const model = resolveRelativeNode(currentDoc, currentDoc, [entityType, modelName]);
    for (const node of descendants(model, [entityType, modelName])) {
      if (node.path[node.path.length - 1] === "$ref") {
        references.push(node);
      }
    }
  }

  const sourceDocObj = await sourceDoc.ReadObject<any>();
  const mappings = Array.from(identitySourceMapping(sourceDoc.key, await sourceDoc.ReadYamlAst()));
  const inputs: DataHandleRead[] = [sourceDoc];
  for (const { node, path } of references) {
    const refPath = node.value as string;
    if (refPath.indexOf("#") === -1) {
      continue; // TODO: could inject entire referenced file here
    }
    const refPathParts = refPath.split("#").filter(s => s.length > 0);
    let fileUri: string | null = null;
    let entityPath = refPath;
    if (refPathParts.length === 2) {
      fileUri = refPathParts[0];
      entityPath = "#" + refPathParts[1];
      node.value = entityPath;
      fileUri = resolveUri(sourceFileUri, fileUri);
      if (!externalFiles[fileUri]) {
        const externalFile = await LoadLiterateYaml(inputScope, fileUri, workingScope.CreateScope(`ext_${Object.getOwnPropertyNames(externalFiles).length}`));
        if (externalFile === null) {
          throw new Error(`File ${fileUri} not found.`);
        }
        externalFiles[fileUri] = externalFile;
      }
    }

    const entityPathParts = entityPath.split("/").filter(s => s.length > 0);
    const referencedEntityType = entityPathParts[1];
    const referencedModelName = entityPathParts[2];

    sourceDocObj[referencedEntityType] = sourceDocObj[referencedEntityType] || {};
    if (visitedEntities.indexOf(entityPath) === -1) {
      visitedEntities.push(entityPath);
      if (sourceDocObj[referencedEntityType][referencedModelName] === undefined) {
        if (fileUri != null) {
          await EnsureCompleteDefinitionIsPresent(inputScope, workingScope, visitedEntities, externalFiles, sourceFileUri, fileUri, referencedEntityType, referencedModelName);
          const extObj = await externalFiles[fileUri].ReadObject<any>();
          inputs.push(externalFiles[fileUri]);
          sourceDocObj[referencedEntityType][referencedModelName] = extObj[referencedEntityType][referencedModelName];
        }
        else {
          await EnsureCompleteDefinitionIsPresent(inputScope, workingScope, visitedEntities, externalFiles, sourceFileUri, currentFileUri, referencedEntityType, referencedModelName);
          const currentObj = await externalFiles[currentFileUri].ReadObject<any>();
          inputs.push(externalFiles[currentFileUri]);
          sourceDocObj[referencedEntityType][referencedModelName] = currentObj[referencedEntityType][referencedModelName];
        }
      } else {
        // throw new Error(`Model definition '${entityPath}' already present`);
      }
    }
  }

  //ensure that all the models that are an allOf on the current model in the external doc are also included
  if (entityType != null && modelName != null) {
    var reference = "#/" + entityType + "/" + modelName;
    const dependentRefs: YAMLNodeWithPath[] = [];
    for (const node of descendants(currentDoc)) {
      const path = node.path;
      if (path.length > 3 && path[path.length - 3] === "allOf" && path[path.length - 1] === "$ref" && (node.node.value as string).indexOf(reference) !== -1) {
        dependentRefs.push(node);
      }
    }
    for (const dependentRef of dependentRefs) {
      //the JSON Path "definitions.ModelName.allOf[0].$ref" provides the name of the model that is an allOf on the current model
      const refs = dependentRef.path;
      const defSec = refs[0];
      const model = refs[1];
      if (typeof defSec === "string" && typeof model === "string" && visitedEntities.indexOf(model) === -1) {
        //recursively check if the model is completely defined.
        await EnsureCompleteDefinitionIsPresent(inputScope, workingScope, visitedEntities, externalFiles, sourceFileUri, currentFileUri, defSec, model);
        const currentObj = await externalFiles[currentFileUri].ReadObject<any>();
        inputs.push(externalFiles[currentFileUri]);
        sourceDocObj[defSec][model] = currentObj[defSec][model];
      }
    }
  }

  // commit back
  const id = (await workingScope.Enum()).length;
  const target = await workingScope.Write(`revision_${id}.yaml`);
  externalFiles[sourceFileUri] = await target.WriteObject(sourceDocObj, mappings, inputs);
}

export async function LoadLiterateSwagger(inputScope: DataStoreViewReadonly, inputFileUri: string, workingScope: DataStoreView): Promise<DataHandleRead> {
  const data = await LoadLiterateYaml(inputScope, inputFileUri, workingScope.CreateScope("yaml"));
  const externalFiles: { [uri: string]: DataHandleRead } = {};
  externalFiles[inputFileUri] = data;
  await EnsureCompleteDefinitionIsPresent(inputScope, workingScope.CreateScope("ref-resolving"), [], externalFiles, inputFileUri);
  return externalFiles[inputFileUri];
}

async function LoadLiterateSwaggers(inputScope: DataStoreViewReadonly, inputFileUris: string[], workingScope: DataStoreView): Promise<DataHandleRead[]> {
  const swaggerScope = workingScope.CreateScope("swagger");
  const rawSwaggers: DataHandleRead[] = [];
  let i = 0;
  for (const inputFileUri of inputFileUris) {
    // read literate Swagger
    const pluginInput = await LoadLiterateSwagger(inputScope, inputFileUri, swaggerScope.CreateScope("deliteralize_" + i));
    rawSwaggers.push(pluginInput);
    i++;
  }
  return rawSwaggers;
}

type ObjectWithPath<T> = { obj: T, path: JsonPath };
function getPropertyValues<T, U>(obj: ObjectWithPath<T>): ObjectWithPath<U>[] {
  const o: T = obj.obj || {};
  return Object.getOwnPropertyNames(o).map(n => getProperty<T, U>(obj, n));
}
function getProperty<T, U>(obj: ObjectWithPath<T>, key: string): ObjectWithPath<U> {
  return { obj: (obj.obj as any)[key], path: obj.path.concat([key]) };
}
function getArrayValues<T>(obj: ObjectWithPath<T[]>): ObjectWithPath<T>[] {
  const o: T[] = obj.obj || [];
  return o.map((x, i) => { return { obj: x, path: obj.path.concat([i]) }; });
}

async function ComposeSwaggers(infoSection: any, inputSwaggers: DataHandleRead[], workingScope: DataStoreView, azureMode: boolean): Promise<DataHandleRead> {
  if (azureMode) {
    // prepare component Swaggers (override info, lift version param, ...)
    for (let i = 0; i < inputSwaggers.length; ++i) {
      const inputSwagger = inputSwaggers[i];
      const outputSwagger = await workingScope.Write(`prepared_${i}.yaml`);
      const swagger = await inputSwagger.ReadObject<any>();
      const mapping: Mapping[] = [];
      const populate: (() => void)[] = []; // populate swagger; deferred in order to simplify source map generation

      // digest "info"
      const info = swagger.info;
      const version = info.version;
      delete info.title;
      delete info.description;
      delete info.version;

      // extract interesting nodes
      const paths = From<ObjectWithPath<any>>([])
        .Concat(getPropertyValues(getProperty({ obj: swagger, path: [] }, "paths")))
        .Concat(getPropertyValues(getProperty({ obj: swagger, path: [] }, "x-ms-paths")));
      const methods = paths.SelectMany(getPropertyValues);
      const parameters =
        methods.SelectMany(method => getArrayValues<any>(getProperty<any, any>(method, "parameters"))).Concat(
          paths.SelectMany(path => getArrayValues<any>(getProperty<any, any>(path, "parameters"))));

      // inline api-version params
      const clientParams = swagger.parameters || {};
      const apiVersionClientParamName = Object.getOwnPropertyNames(clientParams).filter(n => clientParams[n].name === "api-version")[0];
      const apiVersionClientParam = apiVersionClientParamName ? clientParams[apiVersionClientParamName] : null;
      if (apiVersionClientParam) {
        const apiVersionClientParam = clientParams[apiVersionClientParamName];
        const apiVersionParameters = parameters.Where(p => p.obj.$ref === `#/parameters/${apiVersionClientParamName}`);
        for (let apiVersionParameter of apiVersionParameters) {
          delete apiVersionParameter.obj.$ref;

          // forward client param
          populate.push(() => Object.assign(apiVersionParameter.obj, apiVersionClientParam));
          mapping.push(...Array.from(CreateAssignmentMapping(
            apiVersionClientParam, inputSwagger.key,
            ["parameters", apiVersionClientParamName], apiVersionParameter.path,
            "inlining api-version")));

          // make constant
          populate.push(() => apiVersionParameter.obj.enum = [version]);
          mapping.push({
            name: "inlining api-version", source: inputSwagger.key,
            original: { path: [<JsonPathComponent>"info", "version"] },
            generated: { path: apiVersionParameter.path.concat("enum") }
          });
          mapping.push({
            name: "inlining api-version", source: inputSwagger.key,
            original: { path: [<JsonPathComponent>"info", "version"] },
            generated: { path: apiVersionParameter.path.concat("enum", 0) }
          });
        }

        // remove api-version client param
        delete clientParams[apiVersionClientParamName];
      }

      // inline produces/consumes
      for (const pc of ["produces", "consumes"]) {
        const clientPC = swagger[pc];
        if (clientPC) {
          for (const method of methods) {
            if (!method.obj[pc]) {
              populate.push(() => method.obj[pc] = clientPC);
              mapping.push(...Array.from(CreateAssignmentMapping(
                clientPC, inputSwagger.key,
                [pc], method.path.concat([pc]),
                `inlining ${pc}`)));
            }
          }
        }
        delete swagger[pc];
      }

      // finish source map
      mapping.push(...Array.from(identitySourceMapping(inputSwagger.key, toAst(swagger))));

      // populate object
      populate.forEach(f => f());

      // write back
      inputSwaggers[i] = await outputSwagger.WriteObject(swagger, mapping, [inputSwagger]);
    }
  }

  const hwSwagger = await workingScope.Write("swagger.yaml");
  let hSwagger = await mergeYamls(inputSwaggers, hwSwagger);

  // custom info section
  if (infoSection) {
    const hwInfo = await workingScope.Write("info.yaml");
    const hInfo = await hwInfo.WriteObject({ info: infoSection });

    const hwSwagger = await workingScope.Write("swagger_customInfo.yaml");
    hSwagger = await mergeYamls([hSwagger, hInfo], hwSwagger);
  }

  return hSwagger;
}

export async function RunPipeline(configurationUri: string, workingScope: DataStoreView): Promise<{ [name: string]: DataPromise }> {
  // load config
  const hConfig = await LoadLiterateYaml(
    workingScope.CreateScope(KnownScopes.Input).AsFileScopeReadThrough(uri => uri === configurationUri),
    configurationUri,
    workingScope.CreateScope("config"));
  const config = new AutoRestConfigurationManager(await hConfig.ReadObject<AutoRestConfiguration>(), configurationUri);

  // load Swaggers
  const swaggers = await LoadLiterateSwaggers(
    // TODO: unlock further URIs here
    workingScope.CreateScope(KnownScopes.Input).AsFileScopeReadThrough(uri => config.inputFileUris.indexOf(uri) !== -1),
    config.inputFileUris, workingScope.CreateScope("loader"));

  // compose Swaggers
  const swagger = await ComposeSwaggers(config.__specials.infoSectionOverride || {}, swaggers, workingScope.CreateScope("compose"), true);

  return {
    componentSwaggers: MultiPromiseUtility.list(swaggers),
    swagger: MultiPromiseUtility.single(swagger)
  };
}