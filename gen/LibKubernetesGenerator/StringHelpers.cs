using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using NJsonSchema;
using Nustache.Core;

namespace LibKubernetesGenerator
{
    internal class StringHelpers : INustacheHelper
    {
        private readonly GeneralNameHelper generalNameHelper;

        public StringHelpers(GeneralNameHelper generalNameHelper)
        {
            this.generalNameHelper = generalNameHelper;
        }

        public void RegisterHelper()
        {
            Helpers.Register(nameof(ToXmlDoc), ToXmlDoc);
            Helpers.Register(nameof(ToInterpolationPathString), ToInterpolationPathString);
            Helpers.Register(nameof(IfGroupPathParamContainsGroup), IfGroupPathParamContainsGroup);
        }

        private void ToXmlDoc(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            if (arguments != null && arguments.Count > 0 && arguments[0] != null && arguments[0] is string)
            {
                var first = true;

                using (var reader = new StringReader(arguments[0] as string))
                {
                    string line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        foreach (var wline in WordWrap(line, 80))
                        {
                            if (!first)
                            {
                                context.Write("\n");
                                context.Write("        /// ");
                            }
                            else
                            {
                                first = false;
                            }

                            context.Write(SecurityElement.Escape(wline));
                        }
                    }
                }
            }
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

        public void ToInterpolationPathString(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var p = arguments?.FirstOrDefault() as string;
            if (p != null)
            {
                context.Write(Regex.Replace(p, "{(.+?)}", (m) => "{" + generalNameHelper.GetDotNetName(m.Groups[1].Value) + "}"));
            }
        }

        public void IfGroupPathParamContainsGroup(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var p = arguments?.FirstOrDefault() as string;
            if (p?.StartsWith("apis/{group}") == true)
            {
                fn(null);
            }
        }
    }
}
