namespace mos6502.Decoding;

public class MicrocodeRom
{
    protected static Signal CHANGE_STATE(Cycle state) => new()
        { Cycle = state, };
    
    protected static Signal REG_MOVE(Pointer source, Pointer destination) => new()
        { Cycle = Cycle.REG_MOVE, First = source, Second = destination };

    protected static Signal MEM_READ(Pointer[] pair) => new()
        { Cycle = Cycle.MEM_READ, First = pair[0], Second = pair[1] };
    protected static Signal MEM_WRITE(Pointer[] pair) => new()
        { Cycle = Cycle.MEM_WRITE, First = pair[0], Second = pair[1] };

    protected static Signal ALU_COMPUTE(Operation operation, Pointer source) => new()
        { Cycle = Cycle.ALU_EXECUTE, Operation = operation, First = source };

    protected static Signal PAIR_INC(Pointer[] pair) => new()
        { Cycle = Cycle.REG_INC, First = pair[0], Second = pair[1] };
    protected static Signal PAIR_DEC(Pointer[] pair) => new()
        { Cycle = Cycle.REG_INC, First = pair[0], Second = pair[1] };
}