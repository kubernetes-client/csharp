namespace k8s;

internal sealed class QueryBuilder
{
    private List<string> parameters = new List<string>();

    public void Append(string key, params object[] values)
    {
        foreach (var value in values)
        {
            switch (value)
            {
                case int intval:
                    parameters.Add($"{key}={intval}");
                    break;
                case string strval:
                    parameters.Add($"{key}={Uri.EscapeDataString(strval)}");
                    break;
                case bool boolval:
                    parameters.Add($"{key}={(boolval ? "true" : "false")}");
                    break;
                default:
                    // null
                    break;
            }
        }
    }

    public override string ToString()
    {
        if (parameters.Count > 0)
        {
            return "?" + string.Join("&", parameters);
        }

        return "";
    }
}
