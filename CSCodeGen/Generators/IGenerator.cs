using CSCodeGen.Structures;

namespace CSCodeGen.Generators;

public interface IGenerator
{
    IEnumerable<CodeLine> Generate(GenerationContext context);
}