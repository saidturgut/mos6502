namespace mos6502.Decoding.Microcodes;
using Executing.Computing;

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

    protected static Signal ALU_COMPUTE(Action action, Pointer source, Pointer operand) => new()
        { Cycle = Cycle.ALU_COMPUTE, Action = action, First = source, Second = operand };
    protected static Signal SRU_COMPUTE(Action action, Flag mask) => new()
        { Cycle = Cycle.SRU_COMPUTE, Action = action, Mask = mask };

    protected static Signal PAIR_INC(Pointer[] pair) => new()
        { Cycle = Cycle.REG_INC, First = pair[0], Second = pair[1] };
    protected static Signal PAIR_DEC(Pointer[] pair) => new()
        { Cycle = Cycle.REG_INC, First = pair[0], Second = pair[1] };
}