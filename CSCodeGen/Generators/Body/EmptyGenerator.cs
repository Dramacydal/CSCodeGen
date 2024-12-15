using CSCodeGen.Structures;

namespace CSCodeGen.Generators.Body;

public class EmptyGenerator : MethodBodyGenerator
{
    public string Comment { get; set; } = "";

    public override List<CodeLine> Generate(GenerationContext context)
    {
        return new() { CodeLine.EmptyLine };
    }
}
