namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    // ------------------------- FAST MODES ------------------------- //
    
    private static Signal[] IMPLIED => // IMP
    [
    ];

    private static Signal[] IMMEDIATE => // IMM
    [
        REG_COMMIT(Pointer.PCL, Pointer.WL),
        REG_COMMIT(Pointer.PCH, Pointer.ZL),
        PAIR_INC(PC)
    ];
        
    private static Signal[] ZERO_PAGE => // ZP, ZPX, ZPY
    [
        ..READ_IMM,
        REG_COMMIT(Pointer.MDR, Pointer.WL),
    ];

    private static Signal[] ZERO_PAGE_IDX(Pointer index) =>
    [
        ..READ_IMM,
        ..COMMIT_INDEX(Operation.IDX, Pointer.MDR, Pointer.WL, index),
    ];
    
    // ------------------------- SLOW MODES ------------------------- //
    
    private static Signal[] ABSOLUTE => // ABS
    [
        ..READ_IMM,
        REG_COMMIT(Pointer.MDR, Pointer.WL),
        ..READ_IMM,
        REG_COMMIT(Pointer.MDR, Pointer.ZL),
    ];

    private static Signal[] ABSOLUTE_IDX(Pointer index) => // ABS ZP
    [
        ..READ_IMM,
        ..COMMIT_INDEX(Operation.IDX, Pointer.MDR, Pointer.WL, index),
        ..READ_IMM,
        ..COMMIT_INDEX(Operation.CRY, Pointer.MDR, Pointer.ZL, Pointer.NIL),
    ];
    
    private static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE,
        ..INDIRECT_ADDRESS(),
    ];
    
    private static Signal[] INDIRECT_X => // INDX
    [
        ..INDIRECT_POINTER,
        ..COMMIT_INDEX(Operation.IDX, Pointer.WL, Pointer.WL, Pointer.IX),
        ..INDIRECT_ADDRESS(),
    ];

    private static Signal[] INDIRECT_Y => // INDY
    [
        ..INDIRECT_POINTER,
        ..INDIRECT_ADDRESS(),
        ..COMMIT_INDEX(Operation.IDX, Pointer.WL, Pointer.WL, Pointer.IY), 
        ..COMMIT_INDEX(Operation.CRY, Pointer.ZL, Pointer.ZL, Pointer.NIL)
    ];
    
    // ------------------------- MACROS ------------------------- //

    private static Signal[] COMMIT_INDEX(Operation operation, Pointer source, Pointer destination, Pointer index) =>
    [
        ALU_COMPUTE(operation, source, index, Flag.NONE),
        REG_COMMIT(Pointer.TMP, destination),
    ];
    
    private static Signal[] READ_IMM => // IMM
    [
        MEM_READ(PC),
        PAIR_INC(PC),
    ];
    
    private static Signal[] INDIRECT_POINTER =>
    [
        ..READ_IMM,
        REG_COMMIT(Pointer.MDR, Pointer.WL),
    ];
    
    private static Signal[] INDIRECT_ADDRESS() =>
    [
        MEM_READ(WZ),
        REG_COMMIT(Pointer.MDR, Pointer.TMP),
        PAIR_INC(WZ),
        MEM_READ(WZ),
        REG_COMMIT(Pointer.MDR, Pointer.ZL),
        REG_COMMIT(Pointer.TMP, Pointer.WL),
    ];
}