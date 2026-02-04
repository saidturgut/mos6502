namespace mos6502.Decoding.Microcodes;

public partial class Microcode : MicrocodeRom
{
    protected static Signal[] FETCH => [];
    protected static Signal[] NONE = Array.Empty<Signal>();
    protected static Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    protected static Pointer[] SP = [Pointer.SP, Pointer.ZERO];
    protected static Pointer[] WZ = [Pointer.W, Pointer.Z];
    protected static Pointer[] TMP = [Pointer.TMP, Pointer.ZERO];

    protected static Signal[] LD(Pointer destination) =>
    [
        REG_MOVE(Pointer.TMP, destination),
        
    ];
}