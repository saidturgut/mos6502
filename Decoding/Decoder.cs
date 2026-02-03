namespace mos6502.Decoding;

public class Decoder : Microcode
{
    public byte opcode;

    public Pointer operand;
    
    public Signal[] Decode(byte value)
    {
        opcode = value;

        throw new Exception("ILLEGAL OPCODE!!");
    }
    
    public Signal[] FETCH = [];
}

public struct Signal()
{
    public Cycle Cycle = Cycle.IDLE;
    public Pointer First = Pointer.NONE;
    public Pointer Second = Pointer.NONE;
    public Operation Operation;
}

public enum Cycle
{
    IDLE, DECODE, HALT, 
    REG_MOVE, MEM_READ, MEM_WRITE,
    ALU_EXECUTE, REG_INC, REG_DEC,
}

public enum Pointer
{
    PCL, PCH, // PROGRAM COUNTER
    SP, NONE, // STACK POINTER
    A, X, Y, // 8 BIT DATA REGISTERS
    ABL, ABH, DBB, // ABUS AND DBUS DRIVERS
    IR, SR, // INSTRUCTION AND STATUS REGISTERS
    W, Z, TMP, // TEMPORARY REGISTERS
} 

public struct Operation()
{
    public Action Action = Action.NONE;
    public Flag Flags = Flag.NONE;
    public bool CarryIn = false;
}

public enum Action
{
    NONE, ADD, SUB,
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