namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    private static readonly Signal[] NONE = Array.Empty<Signal>();
    
    private static readonly Pointer[] PC = [Pointer.PCL, Pointer.PCH];
    private static readonly Pointer[] SP = [Pointer.SP, Pointer.NIL];
    private static readonly Pointer[] WZ = [Pointer.WL, Pointer.ZL];
    
    public static Signal[] FETCH =>
    [
        ..READ_IMM,
        REG_COMMIT(Pointer.MDR, Pointer.IR),
        CHANGE_STATE(Cycle.DECODE),
    ];
    
    private static Signal CHANGE_STATE(Cycle state) => new()
        { Cycle = state, };
    
    private static Signal REG_COMMIT(Pointer source, Pointer destination) => new()
        { Cycle = Cycle.REG_COMMIT, First = source, Second = destination };

    private static Signal MEM_READ(Pointer[] address) => new()
        { Cycle = Cycle.MEM_READ, First = address[0], Second = address[1] };
    private static Signal MEM_WRITE(Pointer[] address) => new()
        { Cycle = Cycle.MEM_WRITE, First = address[0], Second = address[1] };

    private static Signal ALU_COMPUTE(Operation operation, Pointer source, Pointer operand, Flag mask) => new()
        { Cycle = Cycle.ALU_COMPUTE, Operation = operation, First = source, Second = operand, Mask =  mask };
    private static Signal CHECK_COND(Condition condition) => new()
        { Cycle = Cycle.IDLE, Condition = condition };

    private static Signal PAIR_INC(Pointer[] pair) => new()
        { Cycle = Cycle.PAIR_INC, First = pair[0], };
    private static Signal PAIR_DEC(Pointer[] pair) => new()
        { Cycle = Cycle.PAIR_INC, First = pair[0] };
    
    private static readonly Dictionary<FlagMask, Flag> FlagMasks = new()
    {
        { FlagMask.CZVN, Flag.CARRY | Flag.ZERO | Flag.OVERFLOW | Flag.NEGATIVE },
        { FlagMask.ZVN, Flag.ZERO | Flag.OVERFLOW | Flag.NEGATIVE },
        { FlagMask.CZN, Flag.CARRY | Flag.ZERO | Flag.NEGATIVE },
        { FlagMask.ZN, Flag.ZERO | Flag.NEGATIVE },
    };
}

public enum FlagMask
{
    CZVN, ZVN, CZN, ZN,
}
