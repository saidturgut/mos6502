namespace mos6502.Decoding.Microcodes;
using Executing.Computing;

public partial class Microcode : MicrocodeRom
{
    protected static Signal[] FETCH => [];
    protected static Signal[] NONE = Array.Empty<Signal>();
    protected static Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    protected static Pointer[] SP = [Pointer.SP, Pointer.NIL];
    protected static Pointer[] WZ = [Pointer.W, Pointer.Z];
    protected static Pointer[] W = [Pointer.W, Pointer.NIL];
    protected static Pointer[] TMP = [Pointer.TMP, Pointer.NIL];

    protected static Signal[] LOAD(Pointer destination) =>
    [
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, destination)
    ];
    
    protected static Signal[] STORE(Pointer source) =>
    [
        REG_WRITE(source, Pointer.TMP),
        MEM_WRITE(WZ),
    ];

    protected static Signal[] TRANSFER(Pointer source, Pointer destination, bool flags) =>
    [
        ALU_COMPUTE(Operation.NONE, source, Pointer.NIL, flags ? FlagMasks[FlagMask.ZN] : Flag.NONE),
        REG_WRITE(source, destination),
    ];

    protected static Signal[] ALU(Operation operation, Pointer source, FlagMask mask, bool write) =>
    [
        MEM_READ(WZ),
        ALU_COMPUTE(operation, source, Pointer.TMP, FlagMasks[mask]),
        ..write ? [REG_WRITE(Pointer.TMP, source)] : NONE,
    ];
    
    protected static Signal[] ALU_MEM(Operation operation, FlagMask mask) =>
    [
        MEM_READ(WZ),
        ALU_COMPUTE(operation, Pointer.TMP, Pointer.NIL, FlagMasks[mask]),
        MEM_WRITE(WZ),
    ];
    
    protected static Signal[] ALU_REG(Operation operation, Pointer source, FlagMask mask) =>
    [
        ALU_COMPUTE(operation, source, Pointer.NIL, FlagMasks[mask]),
        REG_WRITE(Pointer.TMP, source),
    ];
    
    protected static Signal[] CLR_SET(bool clr, Flag flag) =>
    [
        ALU_COMPUTE(clr ? Operation.CLR : Operation.SET, Pointer.NIL, Pointer.NIL, flag),
    ];

    private static readonly Dictionary<FlagMask, Flag> FlagMasks = new()
    {
        { FlagMask.CZVN, Flag.CARRY | Flag.ZERO | Flag.OVERFLOW | Flag.NEGATIVE },
        { FlagMask.ZVN, Flag.ZERO | Flag.OVERFLOW | Flag.NEGATIVE },
        { FlagMask.CZN, Flag.CARRY | Flag.ZERO | Flag.NEGATIVE },
        { FlagMask.ZN, Flag.ZERO | Flag.NEGATIVE },
    };
}

public enum FlagMask
{
    CZVN, ZVN, CZN, ZN,
}