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
        
    private static Signal[] ZERO_PAGE(Pointer index) => // ZP, ZPX, ZPY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.MDR, index),
        REG_COMMIT(Pointer.TMP, Pointer.WL),
    ];
    
    // ------------------------- SLOW MODES ------------------------- //
    
    private static Signal[] ABSOLUTE(Pointer index) => // ABS, ABSX, ABSY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.MDR, index),
        REG_COMMIT(Pointer.TMP, Pointer.WL),
        ..READ_IMM,
        ..ADD_CARRY(Pointer.MDR, index),
        REG_COMMIT(Pointer.TMP, Pointer.ZL),
    ];
    
    private static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE(Pointer.NIL),
        ..INDIRECT_ADDRESS(),
    ];
    
    private static Signal[] INDIRECT_X => // INDX
    [
        ..INDIRECT_POINTER,
        ..ADD_INDEX(Pointer.WL, Pointer.IX),
        ..INDIRECT_ADDRESS(),
    ];

    private static Signal[] INDIRECT_Y => // INDY
    [
        ..INDIRECT_POINTER,
        ..INDIRECT_ADDRESS(),
        ..ADD_INDEX(Pointer.WL, Pointer.IY), 
        ..ADD_CARRY(Pointer.ZL, Pointer.IY)
    ];
    
    // ------------------------- MACROS ------------------------- //
    
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
    
    private static Signal[] ADD_INDEX(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.NIL 
            ? [ALU_COMPUTE(Operation.IDX, source, index, Flag.NONE), 
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