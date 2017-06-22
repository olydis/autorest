using System.Linq;
using System.Xml.Linq;
using AutoRest.Core;
using AutoRest.Core.Model;
using AutoRest.CSharp.Model;

using static AutoRest.Core.CodeFragments;
using static AutoRest.Core.CodeFragmentsCs;

namespace AutoRest.CSharp.Code
{
    public static class Enum
    {
        static CodeFragment GenerateEnumDefAsString(EnumTypeCs type) =>
            Class(Modifier.Public | Modifier.Static, $"{type.ClassName}",
                new [] { Concat(
                    type.Values.SelectMany(v => new []{
                        v.Description != null ? XmlSummary(v.Description) : None,
                        Statement(Constant($"public const string {v.MemberName} = \"{v.SerializedName}\""))
                    })
                ) }
            );

        static CodeFragment GenerateEnumValueDef(EnumValue value) => Concat(
                value.Description != null ? XmlSummary(value.Description) : None,
                Attribute("System.Runtime.Serialization.EnumMember", $"Value = \"{value.SerializedName}\""),
                Constant(value.MemberName)
            );
        static CodeFragment GenerateEnumDefAsEnum(EnumTypeCs type) => Concat(
                Attribute("Newtonsoft.Json.JsonConverter", "typeof(Newtonsoft.Json.Converters.StringEnumConverter)"),
                Enum(Modifier.Public, type.ClassName,
                    type.Values.Select(GenerateEnumValueDef)
                ),
                Class(Modifier.Internal | Modifier.Static, $"{type.ClassName}EnumExtension", E(
                    Constant($@"internal static string ToSerializedValue(this {type.ClassName}? value)  =>
    value == null ? null : (({type.ClassName})value).ToSerializedValue();
"),
                    Constant($@"internal static string ToSerializedValue(this {type.ClassName} value)"),
                    Scope(
                        Constant("switch( value )"), NewLine,
                        Scope(
                            Concat(type.Values.Select(v => Concat(
                                Constant($"case {type.ClassName}.{v.MemberName}:"),
                                Indented(Constant($"return \"{v.SerializedName}\";"))
                            )))
                        ),
                        Statement(Constant("return null"))
                    ),
                    Constant($@"internal static {type.ClassName}? Parse{type.ClassName}(this string value)"),
                    Scope(
                        Constant("switch( value )"), NewLine,
                        Scope(
                            Concat(type.Values.Select(v => Concat(
                                Constant($"case \"{v.SerializedName}\":"),
                                Indented(Constant($"return {type.ClassName}.{v.MemberName};"))
                            )))
                        ),
                        Statement(Constant("return null"))
                    )
                ))
            );

        static CodeFragment GenerateEnumFile(Model.EnumTypeCs type) => Concat(
                MultilineComment(Settings.Instance.Header),
                NewLine,
                Namespace($"{Settings.Instance.Namespace}.{Settings.Instance.ModelsName}",
                    NewLine,
                    XmlSummary($"Defines values for {type.Name}."),
                    type.ModelAsString
                        ? GenerateEnumDefAsString(type)
                        : GenerateEnumDefAsEnum(type)
                )
            );

        public static string Generate(Model.EnumTypeCs type) => Execute(GenerateEnumFile(type));
    }
}