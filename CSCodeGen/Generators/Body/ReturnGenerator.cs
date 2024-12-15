using CSCodeGen.Structures;

namespace CSCodeGen.Generators.Body;

public class ReturnGenerator : MethodBodyGenerator
{
    public Optional Value { get; set; } = new();

    public ReturnGenerator()
    {
    }

    public ReturnGenerator(object? value)
    {
        Value = new(value);
    }

    public override List<CodeLine> Generate(GenerationContext context)
    {
        return new()
        {
            context.CreateLine(Value.HasValue ? $"return {Formatter.FormatValue(Value.Value, context)}; " : "return;")
        };
    }
}
