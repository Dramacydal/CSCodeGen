using CSCodeGen.Structures;

namespace CSCodeGen.Generators.Body;

public class ClassCallGenerator : MethodBodyGenerator
{
    public string MethodName { get; set; }
    
    public List<object> Arguments { get; set; } = new();

    public ClassCallGenerator()
    {
    }

    public ClassCallGenerator(string methodName, List<object> arguments)
    {
        MethodName = methodName;
        Arguments = arguments.ToList();
    }

    protected static List<CodeLine> Generate(string methodName, List<object> arguments, GenerationContext context)
    {
        var str = methodName + "(";

        str += string.Join(", ", arguments.Select(_ => Formatter.FormatValue(_, context)));

        str += ");";

        return new()
        {
            context.CreateLine(str),
        };
    }

    public override List<CodeLine> Generate(GenerationContext context)
    {
        return Generate(MethodName, Arguments, context);
    }
}