using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class ClassGenerator : IGenerator
{
    public AccessFlags Access { get; set; } = AccessFlags.None;
    public string Name { get; set; }
    public TypeInfo? Parent { get; set; }
    
    public List<PropertyGenerator> Properties { get; } = new();
    
    public List<MethodGenerator> Methods { get; } = new();

    public ClassGenerator SetParent(Type t)
    {
        Parent = new(t);
        return this;
    }

    public ClassGenerator SetParent(TypeInfo t)
    {
        Parent = t;
        return this;
    }

    public ClassGenerator AddMethod(MethodGenerator method)
    {
        Methods.Add(method);

        return this;
    }

    public ClassGenerator AddProperty(PropertyGenerator propertyGen)
    {
        Properties.Add(propertyGen);

        return this;
    }

    public IEnumerable<CodeLine> Generate(GenerationContext context)
    {
        LineGenerator g = new(context);

        var sigParts = Access.GenerateParts();
        sigParts.Add($"class {Name}");

        if (Parent != null)
            sigParts.Add($": {Parent.Format(context)}");

        g.Create(string.Join(" ", sigParts));
        g.Create("{");

        ++context.IndentLevel;
        foreach (var p in Properties)
        {
            g.Lines.AddRange(p.Generate(context));
            g.CreateEmpty();
        }

        --context.IndentLevel;
        
        ++context.IndentLevel;
        foreach (var m in Methods)
        {
            g.Lines.AddRange(m.Generate(context));
            g.CreateEmpty();
        }

        --context.IndentLevel;

        g.Create("}");


        return g.Lines;
    }
}
