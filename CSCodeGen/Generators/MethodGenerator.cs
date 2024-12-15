using CSCodeGen.Generators.Body;
using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class MethodGenerator
{
    public AccessFlags Access { get; set; } = AccessFlags.None;
    public bool Override { get; set; }
    public TypeInfo ReturnType { get; set; } = new(typeof(void));
    public string Name { get; set; }
    public List<ArgumentInfo> Arguments { get; set; } = new();

    public List<MethodBodyGenerator> BodyItems { get; set; } = new();

    public MethodGenerator SetReturnType(Type t)
    {
        ReturnType = new(t);
        return this;
    }

    public MethodGenerator SetReturnType(TypeInfo t)
    {
        ReturnType = t;
        return this;
    }

    public MethodGenerator AddArgument(Type type, string name) => AddArgument(new TypeInfo(type), name);

    public MethodGenerator AddArgument(Type type, string name, bool nullable) =>
        AddArgument(new TypeInfo(type), name, nullable);

    public MethodGenerator AddArgument(Type type, string name, bool nullable, object? def) =>
        AddArgument(new TypeInfo(type), name, nullable, def);

    public MethodGenerator AddArgument(TypeInfo type, string name) => AddArgument(type, name, false);

    public MethodGenerator AddArgument(TypeInfo type, string name, bool nullable) =>
        AddArgument(new ArgumentInfo()
        {
            Type = type,
            Name = name,
            Nullable = nullable,
            Default = new(),
        });

    public MethodGenerator AddArgument(TypeInfo type, string name, bool nullable, object? def) =>
        AddArgument(new ArgumentInfo()
        {
            Type = type,
            Name = name,
            Nullable = nullable,
            Default = new(def),
        });

    public MethodGenerator AddArgument(ArgumentInfo argument)
    {
        Arguments.Add(argument);

        return this;
    }

    public MethodGenerator AddBodyItem(MethodBodyGenerator methodBodyGenerator)
    {
        BodyItems.Add(methodBodyGenerator);
        return this;
    }

    public IEnumerable<CodeLine> Generate(GenerationContext context)
    {
        var g = new LineGenerator(context);

        var signature = $"{ReturnType.Format(context)} {Name}";

        var parts = Access.GenerateParts();
        if (Override)
            parts.Add("override");
        if (parts.Count > 0)
            signature = string.Join(" ", parts) + " " + signature;

        signature += "(" + string.Join(", ", Arguments.Select(_ => Formatter.FormatArgument(_, context))) + ")";

        g.Create(signature);
        g.Create("{");
        ++context.IndentLevel;
        foreach (var asd in BodyItems)
            g.Lines.AddRange(asd.Generate(context));
        --context.IndentLevel;
        g.Create("}");

        return g.Lines;
    }
}
