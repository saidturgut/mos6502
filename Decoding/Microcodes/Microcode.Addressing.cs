namespace mos6502.Decoding.Microcodes;
using Executing.Computing;

public partial class Microcode
{
    // ------------------------- BYTE OUTPUT ------------------------- //
    
    protected static Signal[] IMPLIED => // IMP
    [
    ];
    
    protected static Signal[] IMMEDIATE => // IMM
    [
        MEM_READ(PC),
        PAIR_INC(PC),
        ..BYTE_ADDRESS(Pointer.TMP),
    ];
    
    protected static Signal[] ZERO_PAGE(Pointer index) => // ZP, ZPX, ZPY
    [
        ..IMMEDIATE,
        ..ADD_INDEX(Pointer.TMP, index),
        MEM_READ(TMP),
        ..BYTE_ADDRESS(Pointer.TMP),
    ];
    
    // ------------------------- WORD OUTPUT ------------------------- //
    
    protected static Signal[] ABSOLUTE(Pointer index) => // ABS, ABSX, ABSY
    [
        ..IMMEDIATE,
        ..ADD_INDEX(Pointer.TMP, index),
        REG_WRITE(Pointer.TMP, Pointer.W),
        ..IMMEDIATE,
        ..ADD_CARRY(Pointer.TMP, index),
        REG_WRITE(Pointer.TMP, Pointer.Z),
    ];
    
    protected static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE(Pointer.NIL),
        ..INDIRECT_ADDRESS(W),
    ];
    
    protected static Signal[] INDIRECT_X => // INDX
    [
        ..INDIRECT_POINTER,
        ..ADD_INDEX(Pointer.TMP, Pointer.X),
        ..INDIRECT_ADDRESS(WZ),
    ];

    protected static Signal[] INDIRECT_Y => // INDY
    [
        ..INDIRECT_POINTER,
        ..INDIRECT_ADDRESS(WZ),
        ..ADD_INDEX(Pointer.W, Pointer.Y), 
        ..ADD_CARRY(Pointer.Z, Pointer.NIL)
    ];
    
    // ------------------------- MACROS ------------------------- //

    private static Signal[] INDIRECT_POINTER =>
    [
        ..IMMEDIATE,
        REG_WRITE(Pointer.TMP, Pointer.W),
        REG_WRITE(Pointer.NIL, Pointer.Z),
    ];
    
    private static Signal[] INDIRECT_ADDRESS(Pointer[] pair) =>
    [
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.W),
        PAIR_INC(pair),
        MEM_READ(WZ),
        REG_WRITE(Pointer.TMP, Pointer.Z),
    ];

    private static Signal[] BYTE_ADDRESS(Pointer source) =>
    [
        REG_WRITE(source, Pointer.W),
        REG_WRITE(Pointer.NIL, Pointer.Z),
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
            ? [ALU_COMPUTE(Operation.ADC, source, Pointer.NIL, Flag.NONE), 
                ..source is not Pointer.TMP ? [REG_WRITE(Pointer.TMP, source)] : NONE]
            : NONE,
    ];
    
    protected static Signal[] RELATIVE => // REL
    [
        ..IMMEDIATE,
    ];
}