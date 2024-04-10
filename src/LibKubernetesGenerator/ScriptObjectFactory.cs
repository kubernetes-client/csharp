using Scriban.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace LibKubernetesGenerator;

internal class ScriptObjectFactory
{
    private readonly List<IScriptObjectHelper> scriptObjectHelpers;

    public ScriptObjectFactory(IEnumerable<IScriptObjectHelper> scriptObjectHelpers)
    {
        this.scriptObjectHelpers = scriptObjectHelpers.ToList();
    }

    public ScriptObject CreateScriptObject()
    {
        var scriptObject = new ScriptObject();
        foreach (var helper in scriptObjectHelpers)
        {
            helper.RegisterHelper(scriptObject);
        }

        return scriptObject;
    }
}
