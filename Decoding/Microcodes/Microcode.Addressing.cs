namespace mos6502.Decoding.Microcodes;

public partial class Microcode
{
    // --------------------- MODES --------------------- //
    
    protected static Signal[] IMPLICIT => // IMP
    [
    ];

    protected static Signal[] ACCUMULATOR => // ACC
    [
        REG_MOVE(Pointer.A, Pointer.TMP)
    ];
    
    protected static Signal[] IMMEDIATE => // IMM
    [
        MEM_READ(PC),
        PAIR_INC(PC),
    ];

    protected static Signal[] ZERO_PAGE(Pointer index) => // ZP
    [
        ..IMMEDIATE,
        ..ADD_INDEX(Pointer.TMP, index),
        MEM_READ(TMP),
    ];

    protected static Signal[] ABSOLUTE(Pointer index) => // ABS
    [
        ..ABSOLUTE_POINTER(index),
        MEM_READ(WZ),
    ];
    
    protected static Signal[] RELATIVE => // REL
    [
        ..IMMEDIATE,
    ];
    
    protected static Signal[] INDIRECT(Pointer index) => // IND
    [
        ..ABSOLUTE_POINTER(Pointer.ZERO),
        ..index is Pointer.X ? INDIRECT_X : NONE,
        MEM_READ(WZ),
        REG_MOVE(Pointer.TMP, Pointer.W),
        PAIR_INC(WZ),
        MEM_READ(WZ),
        ..index is Pointer.Y ? INDIRECT_Y : NONE,
        REG_MOVE(Pointer.TMP, Pointer.Z),
        MEM_READ(WZ),
    ];
    
    // --------------------- UTILIY --------------------- //
    
    private static Signal[] ABSOLUTE_POINTER(Pointer index) =>
    [
        ..IMMEDIATE,
        ..ADD_INDEX(Pointer.TMP, index), // TMP + INDEX
        REG_MOVE(Pointer.TMP, Pointer.W),
        ..IMMEDIATE,
        ..ADD_CARRY(Pointer.TMP, index),
        REG_MOVE(Pointer.TMP, Pointer.Z),
    ];
    
    private static Signal[] INDIRECT_X =>
    [
    ];

    private static Signal[] INDIRECT_Y =>
    [
    ];
    
    private static Signal[] ADD_INDEX(Pointer source, Pointer index) =>
    [
        ..index is not Pointer.ZERO 
            ? [ALU_COMPUTE(Action.ADD, source, index)] 
            : NONE,
    ];

    private static Signal[] ADD_CARRY(Pointer source, Pointer index) =>
    [
       /* ..index is not Pointer.ZERO 
            ? [ALU_COMPUTE(Action.CRY, source, Pointer.ZERO)] 
            : NONE,*/
    ];
}