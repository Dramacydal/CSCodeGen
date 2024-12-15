namespace CSCodeGen;

public static class Extensions
{
    public static List<string> GenerateParts(this AccessFlags flags)
    {
        List<string> parts = new();
        if (flags.HasFlag(AccessFlags.Internal))
            parts.Add("internal");
        else if (flags.HasFlag(AccessFlags.Protected))
            parts.Add("protected");
        else if (flags.HasFlag(AccessFlags.Private))
            parts.Add("private");
        else if (flags.HasFlag(AccessFlags.Public))
            parts.Add("public");

        if (flags.HasFlag(AccessFlags.Static))
            parts.Add("static");

        return parts;
    }
}