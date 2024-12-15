using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class ArgumentInfo
{
    public TypeInfo Type { get; set; }
    public string Name { get; set; }
    public bool Nullable { get; set; }
    public Optional Default { get; set; } = new();

    public ArgumentInfo SetType(Type t)
    {
        Type = new(t);
        return this;
    }

    public ArgumentInfo SetType(TypeInfo t)
    {
        Type = t;
        return this;
    }
}
