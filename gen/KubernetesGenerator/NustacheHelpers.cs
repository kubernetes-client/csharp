namespace KubernetesGenerator
{
    public class NustacheHelpers
    {
        public static void RegisterHelpers()
        {
            // Helpers.Register(nameof(ClassNameHelper.GetClassName), new ClassNameHelper());

            // Helpers.Register(nameof(ToXmlDoc), ToXmlDoc);
        }

        // private static void GetRequestMethod(RenderContext context, IList<object> arguments, IDictionary<string, object> options,
        //     RenderBlock fn, RenderBlock inverse)
        // {
        //     var s = arguments?.FirstOrDefault() as SwaggerOperationMethod?;
        //     if (s != null)
        //     {
        //         context.Write(s.ToString().ToUpper());
        //     }
        // }
    }
}
