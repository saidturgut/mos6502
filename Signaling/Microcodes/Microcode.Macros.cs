namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public partial class Microcode
{
    // ------------------------- MACROS ------------------------- //
    
    private static readonly Signal[] NONE = Array.Empty<Signal>();
    
    private static readonly Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    private static readonly Pointer[] SP = [Pointer.SP, Pointer.NIL];
    private static readonly Pointer[] WZ = [Pointer.WR, Pointer.ZR];
    private static readonly Pointer[] W = [Pointer.WR, Pointer.NIL];
    private static readonly Pointer[] TMP = [Pointer.TMP, Pointer.NIL];
    
    private static Signal[] READ_IMM => // IMM
    [
        MEM_READ(PC),
        PAIR_INC(PC),
    ];
    
    private static Signal[] INDIRECT_POINTER =>
    [
        ..READ_IMM,
        REG_COMMIT(Pointer.TMP, Pointer.WR),
        REG_COMMIT(Pointer.NIL, Pointer.ZR),
    ];
    
    private static Signal[] INDIRECT_ADDRESS(Pointer[] pair) =>
    [
        MEM_READ(WZ),
        REG_COMMIT(Pointer.TMP, Pointer.WR),
        PAIR_INC(pair),
        MEM_READ(WZ),
        REG_COMMIT(Pointer.TMP, Pointer.ZR),
    ];

    private static Signal[] BYTE_ADDRESS(Pointer source) =>
    [
        REG_COMMIT(source, Pointer.WR),
        REG_COMMIT(Pointer.NIL, Pointer.ZR),
    ];
    
    private static Signal[] ADD_INDEX(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.NIL 
            ? [ALU_COMPUTE(Operation.ADD, source, index, Flag.NONE), 
                ..source is not Pointer.TMP ? [REG_COMMIT(Pointer.TMP, source)] : NONE] 
            : NONE,
    ];

    private static Signal[] ADD_CARRY(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.NIL
            ? [ALU_COMPUTE(Operation.CRY, source, Pointer.NIL, Flag.NONE), 
                ..source is not Pointer.TMP ? [REG_COMMIT(Pointer.TMP, source)] : NONE]
            : NONE,
    ];
}