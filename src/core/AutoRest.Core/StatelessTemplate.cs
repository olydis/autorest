using AutoRest.Core.Utilities;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

using static AutoRest.Core.CodeFragments;
using static AutoRest.Core.CodeFragmentsCs;

namespace AutoRest.Core
{
    public static class CodeFragments
    {
        public delegate IndentedStringBuilder CodeFragment(IndentedStringBuilder target);

        public static IEnumerable<T> E<T>(params T[] values) => values;

        public static CodeFragment None => target => target;
        public static CodeFragment NewLine => target => target.WriteLineBreak();
        static CodeFragment Concat2(CodeFragment fragment1, CodeFragment fragment2) => target => fragment2(fragment1(target));
        public static CodeFragment Concat(IEnumerable<CodeFragment> fragments) => fragments.Aggregate(None, Concat2);
        public static CodeFragment Concat(params CodeFragment[] fragments) => Concat(fragments.AsEnumerable());
        public static CodeFragment Join(CodeFragment separator, IEnumerable<CodeFragment> fragments)
            => Concat(fragments.SelectMany((f, i) => i == 0 ? new []{ f } : new []{ separator, f }));
        public static CodeFragment Join(string separator, IEnumerable<CodeFragment> fragments) => Join(Constant(separator), fragments);
        public static CodeFragment Join(string separator, params CodeFragment[] fragments) => Join(separator, fragments.AsEnumerable());
        public static CodeFragment Constant(string s) => target => target.Write(s);
        public static CodeFragment ConstantWrapped(string s) => target => target.WriteWrapped(s);
        public static CodeFragment IndentedWith(string customIndent, params CodeFragment[] fragments) => Concat(
                target => target.Indent(customIndent).EndLine(),
                Concat(fragments),
                target => target.EndLine().Outdent()
            );
        public static CodeFragment Indented(params CodeFragment[] fragments) => IndentedWith(null, fragments);
    }
    public static class CodeFragmentsCs
    {
        [Flags]
        public enum Modifier
        { 
            Public   = 1 << 0,
            Private  = 1 << 1,
            Internal = 1 << 2,
            Static   = 1 << 3
        }

        public static string Execute(CodeFragment fragment) => fragment(new IndentedStringBuilder()).ToString();

        public static CodeFragment Scope(params CodeFragment[] fragments) => Concat(
                Constant("{"), NewLine,
                Indented(Concat(fragments)),
                Constant("}"), NewLine
            );
        public static CodeFragment Statement(CodeFragment f) => Concat(f, Constant(";"), NewLine);
        public static CodeFragment UsingStatement(string ns) => Statement(Constant($"using {ns}"));
        public static CodeFragment MultilineComment(string comment) => IndentedWith("// ", ConstantWrapped(comment));
        public static CodeFragment XmlDoc(XElement doc) => IndentedWith("/// ", ConstantWrapped(doc.ToString()));
        public static CodeFragment XmlSummary(params object[] content) => XmlDoc(new XElement("summary", "\n", content, "\n"));
        public static CodeFragment Attribute(string attributeType, params string[] parameters) => Concat(
                parameters.Length == 0
                    ? Constant($"[{attributeType}]")
                    : Concat(Constant($"[{attributeType}("), Join(", ", parameters.Select(Constant)), Constant(")]")),
                NewLine
            );
        public static CodeFragment Modifiers(Modifier modifiers) => Concat(
                System.Enum
                    .GetValues(typeof(Modifier))
                    .Cast<Modifier>()
                    .Where(m => (modifiers & m) != 0)
                    .Select(m => Constant($"{m.ToString().ToLower()} "))
            );

        public static CodeFragment Namespace(string ns, params CodeFragment[] fragments) => Concat(
                Constant($"namespace {ns}"), NewLine,
                Scope(fragments)
            );
        public static CodeFragment Enum(Modifier modifiers, string name, IEnumerable<CodeFragment> fragments) => Concat(
                Modifiers(modifiers),
                Constant($"enum {name}"),
                NewLine,
                Scope(
                    Join(Concat(Constant(","), NewLine), fragments)
                )
            );
        public static CodeFragment Class(Modifier modifiers, string name, IEnumerable<CodeFragment> fragments) => Concat(
                Modifiers(modifiers),
                Constant($"class {name}"),
                NewLine,
                Scope(
                    Join(NewLine, fragments)
                )
            );
    }
}
