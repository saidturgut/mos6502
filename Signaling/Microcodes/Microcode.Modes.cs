namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    // ------------------------- BYTE OUTPUT ------------------------- //
    
    private static Signal[] IMPLIED => // IMP
    [
    ];

    private static Signal[] IMMEDIATE => // IMM
    [
        REG_COMMIT(Pointer.PCL, Pointer.WR),
        REG_COMMIT(Pointer.PCH, Pointer.ZR),
        PAIR_INC(PC)
    ];
        
    private static Signal[] ZERO_PAGE(Pointer index) => // ZP, ZPX, ZPY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.TMP, index),
        ..BYTE_ADDRESS(Pointer.TMP),
    ];
    
    // ------------------------- WORD OUTPUT ------------------------- //
    
    private static Signal[] ABSOLUTE(Pointer index) => // ABS, ABSX, ABSY
    [
        ..READ_IMM,
        ..ADD_INDEX(Pointer.TMP, index),
        REG_COMMIT(Pointer.TMP, Pointer.WR),
        ..READ_IMM,
        ..ADD_CARRY(Pointer.TMP, index),
        REG_COMMIT(Pointer.TMP, Pointer.ZR),
    ];
    
    private static Signal[] INDIRECT => // IND
    [
        ..ABSOLUTE(Pointer.NIL),
        ..INDIRECT_ADDRESS(W),
    ];
    
    private static Signal[] INDIRECT_X => // INDX
    [
        ..INDIRECT_POINTER,
        ..ADD_INDEX(Pointer.TMP, Pointer.IX),
        ..INDIRECT_ADDRESS(WZ),
    ];

    private static Signal[] INDIRECT_Y => // INDY
    [
        ..INDIRECT_POINTER,
        ..INDIRECT_ADDRESS(WZ),
        ..ADD_INDEX(Pointer.WR, Pointer.IY), 
        ..ADD_CARRY(Pointer.ZR, Pointer.IY)
    ];
}