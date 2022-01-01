namespace Z10Forever;

internal readonly struct Patterns
{
    internal byte[] Original { get; init; }

    internal byte[] Patch { get; init; }

    // internal string Mask { get; init; } in case we will need masked patterns in future updates...
}