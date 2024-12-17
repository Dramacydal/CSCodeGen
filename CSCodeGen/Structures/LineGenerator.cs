namespace CSCodeGen.Structures;

public class LineGenerator(GenerationContext context)
{
    public List<CodeLine> Lines { get; } = new();

    public GenerationContext Context => context;

    public LineGenerator Create(string text)
    {
        Lines.Add(context.CreateLine(text));
        return this;
    }

    public LineGenerator CreateEmpty()
    {
        Lines.Add(CodeLine.EmptyLine);
        return this;
    }

    public LineGenerator Insert(int index, CodeLine line, bool relativeIndent = false)
    {
        if (relativeIndent)
            Lines.Insert(index, new(line.Text, Context.IndentLevel + line.IndentLevel));
        else
            Lines.Insert(index, line);

        return this;
    }

    public LineGenerator Add(CodeLine line, bool relativeIndent = false)
    {
        if (relativeIndent)
            Lines.Add(new(line.Text, Context.IndentLevel + line.IndentLevel));
        else
            Lines.Add(line);

        return this;
    }

    public LineGenerator AddRange(IEnumerable<CodeLine> lines, bool relativeIndent = false)
    {
        foreach (var line in lines)
            Add(line, relativeIndent);

        return this;
    }
}
