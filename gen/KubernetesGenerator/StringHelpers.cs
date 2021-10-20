using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using NJsonSchema;
using Nustache.Core;

namespace KubernetesGenerator
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
            Helpers.Register(nameof(AddCurly), AddCurly);
            Helpers.Register(nameof(EscapeDataString), EscapeDataString);
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

        public void AddCurly(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var s = arguments?.FirstOrDefault() as string;
            if (s != null)
            {
                context.Write("{" + s + "}");
            }
        }

        public void EscapeDataString(RenderContext context, IList<object> arguments,
            IDictionary<string, object> options,
            RenderBlock fn, RenderBlock inverse)
        {
            var name = generalNameHelper.GetDotNetName(arguments[0] as string);
            var type = arguments[1] as JsonObjectType?;

            if (name == "pretty")
            {
                context.Write($"{name}.Value == true ? \"true\" : \"false\"");
                return;
            }

            switch (type)
            {
                case JsonObjectType.String:
                    context.Write($"System.Uri.EscapeDataString({name})");
                    break;
                default:
                    context.Write(
                        $"System.Uri.EscapeDataString(SafeJsonConvert.SerializeObject({name}, SerializationSettings).Trim('\"'))");
                    break;
            }
        }
    }
}
