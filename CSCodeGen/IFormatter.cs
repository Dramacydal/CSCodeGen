using CSCodeGen.Structures;

namespace CSCodeGen;

public interface IFormatter
{
    public bool Format(object value, GenerationContext context, bool multiline, out IEnumerable<CodeLine> result);
}