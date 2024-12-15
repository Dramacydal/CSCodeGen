using CSCodeGen.Structures;

namespace CSCodeGen.Generators.Body;

public class CommentGenerator : MethodBodyGenerator
{
    public string Comment { get; set; } = "";

    public override List<CodeLine> Generate(GenerationContext context)
    {
        return Comment.Split("\n").Select(_ => context.CreateLine("// " + _)).ToList();
    }
}