namespace mos6502.Testing;
using mos6502;

internal static class Run
{
    private static readonly Cpu Cpu = new();

    private static void Main()
    {
        Assembler.Run();
        Cpu.Power();
    }
}