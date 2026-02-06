namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public partial class Microcode
{
    // ------------------------- BYTE OUTPUT ------------------------- //
    
    protected static Signal[] IMPLIED => // IMP
    [
    ];

    protected static Signal[] IMMEDIATE => // IMM
    [
        ..READ_IMM,
        ..BYTE_ADDRESS(Pointer.TMP),
    ];
        
    protected static Signal[] ZERO_PAGE(Pointer index) => // ZP, ZPX, ZPY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.TMP, index),
        MEM_READ(TMP),
        ..BYTE_ADDRESS(Pointer.TMP),
    ];
    
    // ------------------------- WORD OUTPUT ------------------------- //
    
    protected static Signal[] ABSOLUTE(Pointer index) => // ABS, ABSX, ABSY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.TMP, index),
        REG_WRITE(Pointer.TMP, Pointer.WR),
        ..READ_IMM,
        ..ADD_CARRY(Pointer.TMP, index),
        REG_WRITE(Pointer.TMP, Pointer.ZR),
    ];
    
    protected static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE(Pointer.NIL),
        ..INDIRECT_ADDRESS(W),
    ];
    
    protected static Signal[] INDIRECT_X => // INDX
    [
        ..INDIRECT_POINTER,
        ..ADD_INDEX(Pointer.TMP, Pointer.IX),
        ..INDIRECT_ADDRESS(WZ),
    ];

    protected static Signal[] INDIRECT_Y => // INDY
    [
        ..INDIRECT_POINTER,
        ..INDIRECT_ADDRESS(WZ),
        ..ADD_INDEX(Pointer.WR, Pointer.IY), 
        ..ADD_CARRY(Pointer.ZR, Pointer.IY)
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
        REG_WRITE(Pointer.TMP, Pointer.WR),
        REG_WRITE(Pointer.NIL, Pointer.ZR),
    ];
    
    private static Signal[] INDIRECT_ADDRESS(Pointer[] pair) =>
    [
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.WR),
        PAIR_INC(pair),
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.ZR),
    ];

    private static Signal[] BYTE_ADDRESS(Pointer source) =>
    [
        REG_WRITE(source, Pointer.WR),
        REG_WRITE(Pointer.NIL, Pointer.ZR),
    ];
    
    private static Signal[] ADD_INDEX(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.NIL 
            ? [ALU_COMPUTE(Operation.ADD, source, index, Flag.NONE), 
                ..source is not Pointer.TMP ? [REG_WRITE(Pointer.TMP, source)] : NONE] 
            : NONE,
    ];

    private static Signal[] ADD_CARRY(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.NIL
            ? [ALU_COMPUTE(Operation.CRY, source, Pointer.NIL, Flag.NONE), 
                ..source is not Pointer.TMP ? [REG_WRITE(Pointer.TMP, source)] : NONE]
            : NONE,
    ];
}