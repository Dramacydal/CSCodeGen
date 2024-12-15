using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public abstract class MethodBodyGenerator : IGenerator
{
    public abstract IEnumerable<CodeLine> Generate(GenerationContext context);
}