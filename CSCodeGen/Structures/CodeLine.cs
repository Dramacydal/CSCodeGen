namespace CSCodeGen.Structures;

public class CodeLine
{
    public int IndentLevel = 0;

    public string Text { get; set; }

    public CodeLine(string text, int indentLevel = 0)
    {
        Text = text;
        IndentLevel = indentLevel;
    }

    // public override string ToString() => (new string(' ', IndentLevel * 4)) + Text;

    public static CodeLine EmptyLine => new("");
}
