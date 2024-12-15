
namespace CSCodeGen;

[Flags]
public enum AccessFlags
{
    None = 0,
    Private = 1,
    Public = 2,
    Protected = 4,
    Static = 8,
    Internal = 16,
}