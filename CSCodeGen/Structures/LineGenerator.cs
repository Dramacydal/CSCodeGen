namespace CSCodeGen.Structures;

public class LineGenerator(GenerationContext context)
{
    public List<CodeLine> Lines { get; } = new();

    public void Create(string text) => Lines.Add(context.CreateLine(text));
    public void CreateEmpty() => Lines.Add(CodeLine.EmptyLine);
}
