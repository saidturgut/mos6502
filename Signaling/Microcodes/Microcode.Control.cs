namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    // ------------------------- PUSH & PULL ------------------------- //

    private static Signal[] PUSH(Pointer source) =>
    [
        source is Pointer.SR ? ALU_COMPUTE(Operation.PSR, Pointer.SR, Pointer.NIL, Flag.NONE) : REG_COMMIT(source, Pointer.MDR),
        ..source is Pointer.SR ? [REG_COMMIT(Pointer.TMP, Pointer.MDR)] : NONE,
        MEM_WRITE(SP),
        PAIR_DEC(SP),
    ];
    
    private static Signal[] PULL(Pointer destination) =>
    [
        PAIR_INC(SP),
        MEM_READ(SP),
        destination is Pointer.SR ? ALU_COMPUTE(Operation.SRP, Pointer.MDR, Pointer.NIL, Flag.NONE) : REG_COMMIT(Pointer.MDR, destination),
        ..destination is Pointer.SR ? [REG_COMMIT(Pointer.TMP, destination)] : NONE,
    ];
    
    // ------------------------- CONTROL FLOW ------------------------- //

    private static Signal[] JUMP =>
    [
        REG_COMMIT(Pointer.WL, Pointer.PCL),
        REG_COMMIT(Pointer.ZL, Pointer.PCH),
    ];
    
    private static Signal[] CALL =>
    [
        PAIR_DEC(PC),
        ..PUSH(Pointer.PCH),
        ..PUSH(Pointer.PCL),
        ..JUMP,
    ];

    private static Signal[] RETURN(bool rti) =>
    [
        ..rti ? PULL(Pointer.SR) : NONE,
        ..PULL(Pointer.WL),
        ..PULL(Pointer.ZL),
        ..rti ? NONE : [PAIR_INC(WZ)],
        ..JUMP
    ];

    private static Signal[] BREAK =>
    [
        PAIR_INC(PC),
        ..PUSH(Pointer.SR),
        ..PUSH(Pointer.PCH),
        ..PUSH(Pointer.PCL),
        
        ALU_COMPUTE(Operation.SET, Pointer.NIL, Pointer.NIL, Flag.INTERRUPT),
        REG_COMMIT(Pointer.TMP, Pointer.WL), // LOAD 0XFF ON WZ
        REG_COMMIT(Pointer.TMP, Pointer.ZL),
        
        MEM_READ(WZ),
        REG_COMMIT(Pointer.MDR, Pointer.PCH),
        PAIR_DEC(WZ),
        MEM_READ(WZ),
        REG_COMMIT(Pointer.MDR, Pointer.PCL),
    ];
}