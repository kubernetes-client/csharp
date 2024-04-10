using NJsonSchema;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace LibKubernetesGenerator
{
    internal class StringHelpers : IScriptObjectHelper
    {
        private readonly GeneralNameHelper generalNameHelper;

        public StringHelpers(GeneralNameHelper generalNameHelper)
        {
            this.generalNameHelper = generalNameHelper;
        }

        public void RegisterHelper(ScriptObject scriptObject)
        {
            scriptObject.Import(nameof(ToXmlDoc), new Func<string, string>(ToXmlDoc));
            scriptObject.Import(nameof(ToInterpolationPathString), ToInterpolationPathString);
            scriptObject.Import(nameof(IfGroupPathParamContainsGroup), IfGroupPathParamContainsGroup);
        }

        public static string ToXmlDoc(string arg)
        {
            if (arg == null)
            {
                return "";
            }

            var first = true;
            var sb = new StringBuilder();

            using (var reader = new StringReader(arg))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    foreach (var wline in WordWrap(line, 80))
                    {
                        if (!first)
                        {
                            sb.Append("\n");
                            sb.Append("        /// ");
                        }
                        else
                        {
                            first = false;
                        }

                        sb.Append(SecurityElement.Escape(wline));
                    }
                }
            }

            return sb.ToString();
        }

        private static IEnumerable<string> WordWrap(string text, int width)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var processedLine = line.Trim();

                // yield empty lines as they are (probably) intensional
                if (processedLine.Length == 0)
                {
                    yield return processedLine;
                }

                // feast on the line until it's gone
                while (processedLine.Length > 0)
                {
                    // determine potential wrapping points
                    var whitespacePositions = Enumerable
                        .Range(0, processedLine.Length)
                        .Where(i => char.IsWhiteSpace(processedLine[i]))
                        .Concat(new[] { processedLine.Length })
                        .Cast<int?>();
                    var preWidthWrapAt = whitespacePositions.LastOrDefault(i => i <= width);
                    var postWidthWrapAt = whitespacePositions.FirstOrDefault(i => i > width);

                    // choose preferred wrapping point
                    var wrapAt = preWidthWrapAt ?? postWidthWrapAt ?? processedLine.Length;

                    // wrap
                    yield return processedLine.Substring(0, wrapAt);
                    processedLine = processedLine.Substring(wrapAt).Trim();
                }
            }
        }

        public string ToInterpolationPathString(string arg)
        {
            return Regex.Replace(arg, "{(.+?)}", (m) => "{" + generalNameHelper.GetDotNetName(m.Groups[1].Value) + "}");
        }

        public static bool IfGroupPathParamContainsGroup(string arg)
        {
            return arg.StartsWith("apis/{group}");
        }
    }
}
