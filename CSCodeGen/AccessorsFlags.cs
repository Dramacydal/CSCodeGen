namespace CSCodeGen;

[Flags]
public enum AccessorsFlags
{
    None = 0,
    Get = 1,
    PrivateGet = 2,
    ProtectedGet = 4,
    Set = 8,
    PrivateSet = 16,
    ProtectedSet = 32,
    Init = 64,
    PrivateInit = 128,
    ProtectedInit = 256,
    Computed = 512,
}
