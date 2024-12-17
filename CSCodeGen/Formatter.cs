using System.Collections;
using CSCodeGen.Generators;
using CSCodeGen.Structures;

namespace CSCodeGen;

public static class Formatter
{
    private static List<IFormatter> _formatters = new();
    
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
        [typeof(bool)] = "bool",
        [typeof(object)] = "object",
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

        if (t.Type != null && t.Type.IsGenericType)
        {
            var genericArguments = t.Type.GetGenericArguments();

            var typeName = t.Type.Name[..t.Type.Name.IndexOf('`')];
            typeName += "<" + string.Join(", ", genericArguments.Select(_ => FormatType(_, context))) + ">";

            return typeName;
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
        return string.Join(" ", FormatValueMultiline(value, context).Select(_ => _.Text));
    }

    public static IEnumerable<CodeLine> FormatValueMultiline(object? value, GenerationContext context)
    {
        if (value == null)
            return [new("null")];

        foreach (var formatter in _formatters)
            if (formatter.Format(value, context, true, out var result))
                return result;

        if (value is RawString rawString)
            return [new(rawString.Value)];

        if (value.GetType().IsEnum)
            return [new(FormatType(value.GetType(), context) + "." + value.ToString())];

        if (value is string s)
            return [new(FormatString(s))];

        if (value is bool b)
            return [new(b ? "true" : "false")];

        var valueType = value.GetType();
        if (typeof(IList).IsAssignableFrom(valueType))
        {
            var gen = new ObjectInstanceGenerator()
            {
                Type = valueType,
                MultiLine = true,
            };

            foreach (var elem in value as IList)
                gen.AddCollectionInitializer(elem);

            return gen.Generate(context.CreateInherited());
        }

        if (typeof(IDictionary).IsAssignableFrom(valueType))
        {
            var gen = new ObjectInstanceGenerator()
            {
                Type = valueType,
                MultiLine = true,
            };

            var iDict = value as IDictionary;
            foreach (var key in iDict.Keys)
                gen.AddCollectionInitializer(key, iDict[key]);

            return gen.Generate(context.CreateInherited());
        }

        return [new(value.ToString())];
    }

    public static void RegisterFormatter(IFormatter formatter) => _formatters.Add(formatter);

    public static string FormatString(string value, bool forceVerbatim = false)
    {
        if (value.IndexOfAny(['\n', '\r', '"']) == -1)
            if (value.Contains("\\") || forceVerbatim)
                return "@\"" + value.Replace("\"", "\"\"").Replace("\r", "\\r").Replace("\n", "\\n") + "\"";

        return $"\"{EscapeString(value)}\"";
    }

    public enum StringMultilineFormat
    {
        Concat,
        Verbatim,
        Raw
    }

    public static IEnumerable<string> FormatStringMultiline(string value, StringMultilineFormat format)
    {
        var parts = value.Split("\n");

        switch (format)
        {
            case StringMultilineFormat.Concat:
            {
                for (var i = 0; i < parts.Length; ++i)
                {
                    var str = FormatString(parts[i]);
                    str = str.Insert(str.Length - 1, "\\n");

                    if (i != parts.Length - 1)
                        yield return str + " +";
                    else
                        yield return str;
                }

                break;
            }
            case StringMultilineFormat.Verbatim:
            {
                for (var i = 0; i < parts.Length; ++i)
                {
                    var escaped = parts[i].Replace("\r", "").Replace("\"", "\"\"");
                    if (i == 0)
                        yield return "@\"" + escaped;
                    else if (i == parts.Length - 1)
                        yield return escaped + "\"";
                    else
                        yield return escaped;
                }

                break;
            }
            case StringMultilineFormat.Raw:
            {
                var maxLen = 0;
                for (var i = 0; i < parts.Length; ++i)
                {
                    var str = parts[i];

                    for (var j = 0; j < str.Length;)
                    {
                        if (str[j] != '"')
                        {
                            ++j;
                            continue;
                        }

                        var len = 1;

                        for (var k = j + 1; k < str.Length; ++k)
                        {
                            if (str[k] == '"')
                                ++len;
                            else
                                break;
                        }

                        maxLen = Math.Max(maxLen, len);

                        j += len;
                    }
                }

                var key = new string('"', Math.Max(3, maxLen + 1));
                yield return key;
                for (var i = 0; i < parts.Length; ++i)
                    yield return parts[i];
                yield return key;
                break;
            }
        }
    }

    public static string EscapeString(string text) => text.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
}
