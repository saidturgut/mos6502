using mos6502.Executing.Computing;

namespace mos6502.Decoding.Microcodes;

public partial class Microcode : MicrocodeRom
{
    protected static Signal[] FETCH => [];
    protected static Signal[] NONE = Array.Empty<Signal>();
    protected static Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    protected static Pointer[] SP = [Pointer.SP, Pointer.ZERO];
    protected static Pointer[] WZ = [Pointer.W, Pointer.Z];
    protected static Pointer[] W = [Pointer.W, Pointer.ZERO];
    protected static Pointer[] TMP = [Pointer.TMP, Pointer.ZERO];

    protected static Signal[] LOAD(Pointer destination) =>
    [
        MEM_READ(WZ),
        REG_MOVE(Pointer.TMP, destination)
    ];
    
    protected static Signal[] STORE(Pointer source) =>
    [
        REG_MOVE(source, Pointer.TMP),
        MEM_WRITE(WZ),
    ];

    protected static Signal[] TRANSFER(Pointer source, Pointer destination, bool flags) =>
    [
        REG_MOVE(source, destination),
        SRU_COMPUTE(Action.SET, flags ? Flag.NEGATIVE | Flag.ZERO : Flag.NONE)
    ];
}