namespace CSCodeGen.Structures;

public class CodeLine
{
    public int IndentLevel = 0;
    
    public string Text { get; set; }

    public override string ToString() => Text;
    
    public static CodeLine EmptyLine => new() { Text = "" };
}
