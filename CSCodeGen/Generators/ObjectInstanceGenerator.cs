using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public class ObjectInstanceGenerator : IGenerator
{
    public class PropertyInitializerInfo
    {
        public string PropertyName { get; set; }
        public object? Value { get; set; }
    }

    public Type Type { get; set; }

    public List<object?> ContructorArgs = new();

    public bool MultiLine { get; set; }

    public List<PropertyInitializerInfo> PropertyInitializers = new();

    public List<KeyValuePair<object?, object?>> CollectionInitializers = new();

    public IEnumerable<CodeLine> Generate(GenerationContext context)
    {
        var g = new LineGenerator(context);

        var str = $"new {Formatter.FormatType(Type, context)}";
        if (ContructorArgs.Count > 0)
            str += "(" + string.Join(", ", ContructorArgs.Select(_ => Formatter.FormatValue(_, context))) + ")";

        g.Create(str);

        if (PropertyInitializers.Count > 0)
        {
            g.Create("{");
            ++context.IndentLevel;
            for (var i = 0; i < PropertyInitializers.Count; ++i)
            {
                var propFormatted = Formatter.FormatValueMultiline(PropertyInitializers[i].Value, context).ToList();
                propFormatted[0].Text = $"{PropertyInitializers[i].PropertyName} = {propFormatted[0].Text}";
                propFormatted[^1].Text += (i != PropertyInitializers.Count - 1 ? "," : "");

                if (MultiLine)
                {
                    g.Create(propFormatted[0].Text);
                    g.AddRange(propFormatted.Skip(1), true);
                }
                else
                    g.Create(string.Join(" ", propFormatted.Select(_ => _.Text)));
            }

            --context.IndentLevel;
            g.Create("}");
        }
        else if (CollectionInitializers.Count > 0)
        {
            g.Create("{");
            ++context.IndentLevel;
            for (var i = 0; i < CollectionInitializers.Count; ++i)
            {
                var collFormatted = Formatter.FormatValueMultiline(CollectionInitializers[i].Value, context).ToList();
                collFormatted[0].Text = (CollectionInitializers[i].Key != null
                    ? "[" + Formatter.FormatValue(CollectionInitializers[i].Key, context) + "] = "
                    : "") + collFormatted[0].Text;
                collFormatted[^1].Text += (i != CollectionInitializers.Count - 1 ? "," : "");

                if (MultiLine)
                {
                    g.Create(collFormatted[0].Text);
                    g.AddRange(collFormatted.Skip(1), true);
                }
                else
                    g.Create(string.Join(" ", collFormatted.Select(_ => _.Text)));
            }

            --context.IndentLevel;
            g.Create("}");
        }

        return MultiLine ? g.Lines : [context.CreateLine(string.Join(" ", g.Lines.Select(_ => _.Text)))];
    }

    public ObjectInstanceGenerator AddConstructorArg(object? value)
    {
        ContructorArgs.Add(value);
        return this;
    }

    public ObjectInstanceGenerator AddPropertyInitializer(string propertyName, object? value)
    {
        PropertyInitializers.Add(new() { PropertyName = propertyName, Value = value });
        return this;
    }

    public ObjectInstanceGenerator AddCollectionInitializer(object? value)
    {
        CollectionInitializers.Add(new(null, value));
        return this;
    }

    public ObjectInstanceGenerator AddCollectionInitializer(object key, object? value)
    {
        CollectionInitializers.Add(new(key, value));
        return this;
    }
}
