// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Perks.JsonRPC;
using AutoRest.Core.Utilities;
using AutoRest.Swagger;
using AutoRest.Core.Extensibility;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.Simplify;

public class Generator : NewPlugin
{
  private string codeGenerator;

  public Generator(Connection connection, string sessionId) : base(connection, sessionId)
  { }

  protected override async Task<bool> ProcessInternal()
  {
    new Settings
    {
      Namespace = await GetValue("namespace"),
      ClientName = await GetValue("clientNameOverride")
    };
    var codeGenerator = await GetValue("codeGenerator");

    var files = await ListInputs();
    if (files.Length != 1)
    {
      return false;
    }

    var plugin = ExtensionsLoader.GetPlugin(codeGenerator);
    var modelAsJson = await ReadFile(files[0]);

    using (plugin.Activate())
    {
        var codeModel = plugin.Serializer.Load(modelAsJson);
        codeModel = plugin.Transformer.TransformCodeModel(codeModel);
        plugin.CodeGenerator.Generate(codeModel).GetAwaiter().GetResult();
    }

    // TODO: extract to own plugin
    if (codeGenerator.IndexOf("csharp", StringComparison.OrdinalIgnoreCase) > -1)
    {
        new CSharpSimplifier().Run().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    // write out files
    var outFS = Settings.Instance.FileSystemOutput;
    var outFiles = outFS.GetFiles("", "*", System.IO.SearchOption.AllDirectories);
    foreach (var outFile in outFiles)
    {
        WriteFile(outFile, outFS.ReadAllText(outFile), null);
    }

    return true;
  }
}