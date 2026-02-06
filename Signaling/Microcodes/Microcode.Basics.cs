namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    // ------------------------- MOVE OPERATIONS ------------------------- //

    private static Signal[] LOAD(Pointer destination) =>
    [
        MEM_READ(WZ),
        REG_COMMIT(Pointer.TMP, destination),
        ALU_COMPUTE(Operation.NONE, destination, Pointer.NIL, FlagMasks[FlagMask.ZN]),
    ];
    
    private static Signal[] STORE(Pointer source) =>
    [
        REG_COMMIT(source, Pointer.TMP),
        MEM_WRITE(WZ),
    ];

    private static Signal[] TRANSFER(Pointer source, Pointer destination, bool flags) =>
    [
        ALU_COMPUTE(Operation.NONE, source, Pointer.NIL, flags ? FlagMasks[FlagMask.ZN] : Flag.NONE),
        REG_COMMIT(source, destination),
    ];

    // ------------------------- ALU OPERATIONS ------------------------- //
    
    private static Signal[] ALU(Operation operation, Pointer source, FlagMask mask, bool write) =>
    [
        MEM_READ(WZ),
        ALU_COMPUTE(operation, source, Pointer.TMP, FlagMasks[mask]),
        ..write ? [REG_COMMIT(Pointer.TMP, source)] : NONE,
    ];
    
    private static Signal[] ALU_MEM(Operation operation, FlagMask mask) =>
    [
        MEM_READ(WZ),
        ALU_COMPUTE(operation, Pointer.TMP, Pointer.NIL, FlagMasks[mask]),
        MEM_WRITE(WZ),
    ];
    
    private static Signal[] ALU_REG(Operation operation, Pointer source, FlagMask mask) =>
    [
        ALU_COMPUTE(operation, source, Pointer.NIL, FlagMasks[mask]),
        REG_COMMIT(Pointer.TMP, source),
    ];

    private static Signal[] BRANCH(Condition condition) =>
    [
        CHECK_COND(condition),
        ..ADD_INDEX(Pointer.PCL, Pointer.WR),
        ..ADD_CARRY(Pointer.PCH, Pointer.ZR),
    ];
    
    private static Signal[] FLAG(bool clr, Flag flag) =>
    [
        ALU_COMPUTE(clr ? Operation.CLR : Operation.SET, Pointer.NIL, Pointer.NIL, flag),
    ];
}