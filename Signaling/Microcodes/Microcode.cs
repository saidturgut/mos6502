namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public partial class Microcode : MicrocodeRom
{
    protected static Signal[] FETCH => [];
    protected static Signal[] NONE = Array.Empty<Signal>();
    protected static Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    protected static Pointer[] SP = [Pointer.SP, Pointer.NIL];
    protected static Pointer[] WZ = [Pointer.WR, Pointer.ZR];
    protected static Pointer[] W = [Pointer.WR, Pointer.NIL];
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

    protected static Signal[] BRANCH(Condition condition) =>
    [
        CHECK_COND(condition),
        ..ADD_INDEX(Pointer.PCL, Pointer.WR),
        ..ADD_CARRY(Pointer.PCH, Pointer.ZR),
    ];

    protected static Signal[] PUSH(Pointer source) =>
    [
        source is Pointer.SR ? ALU_COMPUTE(Operation.PSR, Pointer.SR, Pointer.NIL, Flag.NONE) : REG_WRITE(source, Pointer.TMP),
        MEM_WRITE(SP),
        PAIR_DEC(SP),
    ];
    
    protected static Signal[] PULL(Pointer destination) =>
    [
        PAIR_INC(SP),
        MEM_READ(SP),
        ..destination is Pointer.SR ? [ALU_COMPUTE(Operation.SRP, Pointer.TMP, Pointer.NIL, Flag.NONE)] : NONE,
        REG_WRITE(Pointer.TMP, destination),
    ];

    protected static Signal[] JUMP =>
    [
        REG_WRITE(Pointer.WR, Pointer.PCL),
        REG_WRITE(Pointer.ZR, Pointer.PCH),
    ];
    
    protected static Signal[] CALL =>
    [
        PAIR_DEC(PC),
        ..PUSH(Pointer.PCH),
        ..PUSH(Pointer.PCL),
        ..JUMP,
    ];

    protected static Signal[] RETURN(bool rti) =>
    [
        ..rti ? PULL(Pointer.SR) : NONE,
        ..PULL(Pointer.WR),
        ..PULL(Pointer.ZR),
        ..rti ? NONE : [PAIR_INC(WZ)],
        ..JUMP
    ];

    protected static Signal[] BREAK =>
    [
        PAIR_INC(PC),
        ..PUSH(Pointer.SR),
        ..PUSH(Pointer.PCH),
        ..PUSH(Pointer.PCL),
        ALU_COMPUTE(Operation.SET, Pointer.NIL, Pointer.NIL, Flag.INTERRUPT),
        REG_WRITE(Pointer.TMP, Pointer.WR), // LOAD 0XFF ON WZ
        REG_WRITE(Pointer.TMP, Pointer.ZR),
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.PCH),
        PAIR_DEC(WZ),
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.PCL),
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
