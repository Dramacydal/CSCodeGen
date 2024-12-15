using CSCodeGen.Generators;
using CSCodeGen.Structures;

namespace CSCodeGen;

public static class Formatter
{
    private static readonly Dictionary<Type, string> _explicitTypeNameDict = new()
    {
        [typeof(string)] = "string",
        [typeof(char)] = "char",
        [typeof(byte)] = "byte",
        [typeof(sbyte)] = "sbyte",
        [typeof(ushort)] = "ushort",
        [typeof(short)] = "short",
        [typeof(uint)] = "uint",
        [typeof(int)] = "int",
        [typeof(ulong)] = "ulong",
        [typeof(long)] = "long",
        [typeof(float)] = "float",
        [typeof(double)] = "double",
        [typeof(decimal)] = "decimal",
        [typeof(void)] = "void"
    };

    public static string FormatType(Type t, GenerationContext context, bool ignoreAlias = false) => FormatType(new TypeInfo(t), context, ignoreAlias);

    public static string FormatType(TypeInfo t, GenerationContext context, bool ignoreAlias = false)
    {
        if (t.Type != null && _explicitTypeNameDict.TryGetValue(t.Type, out var val))
            return val;

        context.RegisterType(t);

        if (!ignoreAlias)
        {
            var alias = context.GetAlias(t);
            if (!string.IsNullOrEmpty(alias))
                return alias;
        }

        return t.Name;
    }

    public static string FormatWithReflected(Type type)
    {
        return type.ReflectedType != null ? FormatWithReflected(type.ReflectedType) + "." + type.Name : type.Name;
    }

    public static string FormatArgument(ArgumentInfo argument, GenerationContext context)
    {
        var parts = new List<string>();

        parts.Add(argument.Type.Format(context) + (argument.Nullable ? "?" : ""));
        parts.Add(argument.Name);
        if (argument.Default.HasValue)
        {
            parts.Add("=");
            parts.Add(FormatValue(argument.Default.Value, context));
        }

        return string.Join(" ", parts);
    }

    public static string FormatValue(object? value, GenerationContext context)
    {
        if (value == null)
            return "null";

        if (value is RawString rawString)
            return rawString.Value;

        if (value.GetType().IsEnum)
            return FormatType(value.GetType(), context) + "." + value.ToString();

        if (value is string s)
            return FormatString(s);
        
        if (value is bool b)
            return b ? "true" : "false";

        return value.ToString();
    }

    public static string FormatString(string value) => $"\"{EscapeString(value)}\"";

    public static string EscapeString(string text) => text.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
}
