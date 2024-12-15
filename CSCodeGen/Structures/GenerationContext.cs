namespace CSCodeGen.Structures;

public class GenerationContext
{
    public int IndentLevel { get; set; } = 0;
    public string Namespace { get; set; }
    private List<KeyValuePair<TypeInfo, string>> _registeredTypes = new();

    public List<string> BuildUsings()
    {
        var list = _registeredTypes.Where(p => string.IsNullOrEmpty(p.Value));

        return list.Select(p => p.Key.Namespace).Distinct().ToList();
    }

    public List<KeyValuePair<string, string>> BuildAliases()
    {
        var list = _registeredTypes.Where(p => !string.IsNullOrEmpty(p.Value));

        return list.Select(p =>
            new KeyValuePair<string, string>($"{p.Key.Namespace}.{p.Key.Name}", p.Value)).ToList();
    }

    public CodeLine CreateLine(string text = "") => new CodeLine() { Text = text, IndentLevel = IndentLevel };

    public void RegisterType(Type t, string alias = "") => RegisterType(new TypeInfo(t), alias);

    public void RegisterType(TypeInfo t, string alias = "")
    {
        if (_registeredTypes.Any(_ => _.Key.Namespace == t.Namespace && _.Key.Name == t.Name))
            return;

        if (!string.IsNullOrEmpty(alias))
        {
            _registeredTypes.Add(new(t, alias));
            return;
        }

        var cntSame = _registeredTypes.Count(_ => _.Key.Name == t.Name);
        if (cntSame > 0)
            _registeredTypes.Add(new(t, t.Name + (cntSame + 1)));
        else
            _registeredTypes.Add(new(t, ""));
    }

    public string GetAlias(TypeInfo type)
    {
        var info = _registeredTypes.FirstOrDefault(_ => _.Key.Name == type.Name);

        return info.Key != null ? info.Value : "";
    }
}
