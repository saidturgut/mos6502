namespace mos6502.Decoding.Microcodes;

public partial class Microcode
{
    // ------------------------- BYTE OUTPUT ------------------------- //
    
    protected static Signal[] IMPLICIT => // IMP
    [
    ];

    protected static Signal[] ACCUMULATOR => // ACC
    [
        ..BYTE_ADDRESS(Pointer.A),
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
        REG_MOVE(Pointer.TMP, Pointer.W),
        ..IMMEDIATE,
        ..ADD_CARRY(Pointer.TMP, index),
        REG_MOVE(Pointer.TMP, Pointer.Z),
    ];
    
    protected static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE(Pointer.ZERO),
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
        ..ADD_CARRY(Pointer.Z, Pointer.ZERO)
    ];
    
    // ------------------------- MICROCODES ------------------------- //

    private static Signal[] INDIRECT_POINTER =>
    [
        ..IMMEDIATE,
        REG_MOVE(Pointer.TMP, Pointer.W),
        REG_MOVE(Pointer.ZERO, Pointer.Z),
    ];
    
    private static Signal[] INDIRECT_ADDRESS(Pointer[] pair) =>
    [
        MEM_READ(WZ),
        REG_MOVE(Pointer.TMP, Pointer.W),
        PAIR_INC(pair),
        MEM_READ(WZ),
        REG_MOVE(Pointer.TMP, Pointer.Z),
    ];

    private static Signal[] BYTE_ADDRESS(Pointer source) =>
    [
        REG_MOVE(source, Pointer.W),
        REG_MOVE(Pointer.ZERO, Pointer.Z),
    ];
    
    private static Signal[] ADD_INDEX(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.ZERO 
            ? [ALU_COMPUTE(Action.ADD, source, index)] 
            : NONE,
    ];

    private static Signal[] ADD_CARRY(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.ZERO
            ? [ALU_COMPUTE(Action.CRY, source, Pointer.ZERO)]
            : NONE,
    ];
    
    protected static Signal[] RELATIVE => // REL
    [
        ..IMMEDIATE,
    ];
}