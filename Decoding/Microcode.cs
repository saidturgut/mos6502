namespace mos6502.Decoding;

public class Microcode : MicrocodeRom
{
    public Signal[] IMPLICIT = // IMP
    [
    ];

    public Signal[] ACCUMULATOR = // ACC
    [
        REG_MOVE(Pointer.A, Pointer.DBB)
    ];
    
    public Signal[] IMMEDIATE => // IMM
    [
        MEM_READ(PC),
        PAIR_INC(PC),
    ];

    public Signal[] ZERO_PAGE(Pointer index) => //ZP
    [
        ..IMMEDIATE,
        ..ADD_INDEX(index),
        MEM_READ(PC),
    ];
    
    public Signal[] ABSOLUTE(Pointer index) => // ABS
    [
        ..IMMEDIATE,
        REG_MOVE(Pointer.TMP, Pointer.W),
        ..IMMEDIATE,
        REG_MOVE(Pointer.TMP, Pointer.Z),
        ..ADD_INDEX(index),
        MEM_READ(WZ),
    ];

    public Signal[] RELATIVE => // REL
    [
        
    ];
    
    public Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    public Pointer[] SP = [Pointer.SP, Pointer.NONE];
    public Pointer[] WZ = [Pointer.W, Pointer.Z];
    
    public Signal[] ADD_INDEX(Pointer index) =>
    [
        ..index is not Pointer.NONE
            ? [ALU_COMPUTE(new Operation { Action = Action.ADD }, index)]
            : Array.Empty<Signal>(),
    ];
}