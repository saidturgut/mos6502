namespace mos6502.Decoding;
using Executing.Computing;
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
    public Pointer First = Pointer.NIL;
    public Pointer Second = Pointer.NIL;
    public Operation Operation = Operation.NONE;
    public Condition Condition = Condition.NONE;
    public Flag Mask = Flag.NONE;
}

public enum Cycle
{
    IDLE, DECODE, HALT, 
    REG_WRITE, MEM_READ, MEM_WRITE,
    ALU_COMPUTE, SR_COMPUTE, 
    PAIR_INC, PAIR_DEC,
}

public enum Pointer
{
    PCL, PCH, // PROGRAM COUNTER
    SP, NIL, // STACK POINTER
    ACC, IX, IY, // 8 BIT DATA REGISTERS
    WR, ZR, TMP, // TEMPORARY REGISTERS
    IR, SR, // OPCODE AND STATUS REGISTERS
} 
