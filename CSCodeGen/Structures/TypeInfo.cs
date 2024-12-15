namespace CSCodeGen.Structures;

public class TypeInfo
{
    public Type? Type { get; }
    public string Name { get; set; }
    public string? Namespace { get; set; }
    
    public TypeInfo(Type type)
    {
        Type = type;
        Name = Formatter.FormatWithReflected(type);
        Namespace = type.Namespace;
    }

    public TypeInfo(string name, string ns = "")
    {
        Name = name;
        Namespace = ns;
    }

    public string Format(GenerationContext context) => Formatter.FormatType(this, context);
}
