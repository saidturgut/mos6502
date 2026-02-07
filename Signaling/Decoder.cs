namespace mos6502.Signaling;
using Executing.Computing;
using Microcodes;

public class Decoder
{
    private readonly Signal[][] Table = Microcode.OpcodeRom();

    public readonly Signal[] Fetch = Microcode.FETCH;

    public Signal[] Decode(byte opcode)
    {
        var decoded = Table[opcode];
        
        if (decoded == Array.Empty<Signal>()) 
            throw new Exception($"ILLEGAL OPCODE \"{opcode}\"");

        return decoded;
        
        Console.WriteLine("********");
        foreach (var VARIABLE in Table[opcode]) Console.WriteLine(VARIABLE.Cycle);
        Console.WriteLine("********");
        
        return decoded;
    }
}

public struct Signal()
{
    public string Name = "";
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
    REG_COMMIT, MEM_READ, MEM_WRITE,
    ALU_COMPUTE, PAIR_INC, PAIR_DEC,
}

public enum Pointer
{
    PCL, PCH, // PROGRAM COUNTER
    SP, NIL, // STACK POINTER
    ACC, IX, IY, // 8 BIT DATA REGISTERS
    WL, ZL, TMP, MDR, // TEMPORARY LATCHES
    IR, SR, // OPCODE AND STATUS REGISTERS
} 
