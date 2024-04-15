using Scriban.Runtime;

namespace LibKubernetesGenerator;

internal interface IScriptObjectHelper
{
    void RegisterHelper(ScriptObject scriptObject);
}
