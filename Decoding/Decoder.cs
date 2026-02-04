namespace mos6502.Decoding;
using Microcodes;

public class Decoder : DecoderRom
{
    public void Init()
    {
        
    }

    public Signal[] Decode(byte opcode) 
        => Table[opcode];

    public Signal[] Fetch()
        => FETCH;
}

public struct Signal()
{
    public Cycle Cycle = Cycle.IDLE;
    public Pointer First = Pointer.ZERO;
    public Pointer Second = Pointer.ZERO;
    public Action Action = Action.NONE;
    public Flag Mask = Flag.NONE;
}

public enum Cycle
{
    IDLE, DECODE, HALT, 
    REG_MOVE, MEM_READ, MEM_WRITE,
    ALU_COMPUTE, SRU_COMPUTE, 
    REG_INC, REG_DEC,
}

public enum Pointer
{
    PCL, PCH, // PROGRAM COUNTER
    SP, ZERO, // STACK POINTER
    A, X, Y, // 8 BIT DATA REGISTERS
    W, Z, TMP, // TEMPORARY REGISTERS
    IR, SR, // OPCODE AND STATUS REGISTERS
} 

public enum Action
{
    NONE, ADD, SUB, IND,
}

[Flags]
public enum Flag
{
    NONE = 0,
    CARRY = 1 << 0,
    ZERO = 1 << 1,
    INTERRUPT = 1 << 2,
    DECIMAL = 1 << 3,
    BREAK = 1 << 4,
    UNUSED = 1 << 5,
    OVERFLOW = 1 << 6,
    NEGATIVE = 1 << 7,
}