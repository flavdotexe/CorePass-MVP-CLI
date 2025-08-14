using System;

namespace CorePass.Core.Model;

public sealed record KdfParams(int MemoryMB, int Iterations, int Parallelism)
{
    public static KdfParams DefaultStrong()
        => new(Math.Clamp(Environment.Is64BitProcess ? 384 : 192, 128, 1024), 4, Math.Min(Environment.ProcessorCount, 4));
}
