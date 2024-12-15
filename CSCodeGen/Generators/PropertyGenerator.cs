using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class PropertyGenerator : IGenerator
{
    public AccessFlags Access { get; set; } = AccessFlags.Public;
    public bool Override { get; set; }
    public TypeInfo Type { get; set; }
    public string Name { get; set; }
    public AccessorsFlags Accessors { get; set; } = AccessorsFlags.None;

    public Optional Value { get; set; } = new();

    public PropertyGenerator SetType(Type t)
    {
        Type = new(t);
        return this;
    }

    public PropertyGenerator SetType(TypeInfo t)
    {
        Type = t;
        return this;
    }

    public IEnumerable<CodeLine> Generate(GenerationContext context)
    {
        var g = new LineGenerator(context);

        var signature = $"{Type.Format(context)} {Name}";
        
        var parts = Access.GenerateParts();
        if (Override)
            parts.Add("override");
        if (parts.Count > 0)
            signature = string.Join(" ", parts) + " " + signature;

        List<string> setterParts = new();
        if (!Accessors.HasFlag(AccessorsFlags.Computed))
        {
            if (Accessors.HasFlag(AccessorsFlags.Get))
            {
                if (Accessors.HasFlag(AccessorsFlags.PrivateGet))
                    setterParts.Add("private get;");
                else if (Accessors.HasFlag(AccessorsFlags.ProtectedGet))
                    setterParts.Add("protected get;");
                else
                    setterParts.Add("get;");
            }

            if (Accessors.HasFlag(AccessorsFlags.Init))
            {
                if (Accessors.HasFlag(AccessorsFlags.PrivateInit))
                    setterParts.Add("private init;");
                else if (Accessors.HasFlag(AccessorsFlags.ProtectedInit))
                    setterParts.Add("protected init;");
                else
                    setterParts.Add("init;");
            }
            else if (Accessors.HasFlag(AccessorsFlags.Set))
            {
                if (Accessors.HasFlag(AccessorsFlags.PrivateSet))
                    setterParts.Add("private set;");
                else if (Accessors.HasFlag(AccessorsFlags.ProtectedSet))
                    setterParts.Add("protected set;");
                else
                    setterParts.Add("set;");
            }
        }

        if (setterParts.Count > 0)
            signature += " { " + string.Join(" ", setterParts) + " }";

        if (Value.HasValue)
        {
            if (Accessors.HasFlag(AccessorsFlags.Computed))
                signature += " => ";
            else
                signature += " = ";

            signature += Formatter.FormatValue(Value.Value, context) + ";";
        }
        else if (setterParts.Count == 0)
            signature += ";";

        g.Create(signature);

        return g.Lines;
    }
}