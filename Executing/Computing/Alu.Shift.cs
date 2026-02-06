using mos6502.Signaling.Microcodes;

namespace mos6502.Executing.Computing;
using Signaling;

public partial class Alu
{
    private static AluOutput ASL(AluInput input)
    {
        AluOutput output = new() 
            { Result = (byte)((input.A << 1) & 0xFF) };
        if (Carry(input.A, 7)) output.Flags |= (byte)Flag.CARRY;
        return output;
    }
    private static AluOutput LSR(AluInput input)
    {
        AluOutput output = new()
            { Result = (byte)(input.A >> 1) };
        if (Carry(input.A, 0)) output.Flags |= (byte)Flag.CARRY;
        return output;
    }
    private static AluOutput ROL(AluInput input)
    {
        AluOutput output = new()
            { Result = (byte)(((input.A << 1) | input.C) & 0xFF) };
        if (Carry(input.A, 7)) output.Flags |= (byte)Flag.CARRY;
        return output;
    }
    private static AluOutput ROR(AluInput input)
    {
        AluOutput output = new()
            { Result = (byte)((input.A >> 1) | (input.C << 7)) };
        if (Carry(input.A, 0)) output.Flags |= (byte)Flag.CARRY;
        return output;
    }
    
    private static AluOutput BIT(AluInput input)
    {
        AluOutput output = new()
            { Result = (byte)(input.A & input.B) };
        
        if(output.Result == 0) output.Flags |= (byte)Flag.ZERO;
        if((input.B & 0x80) != 0) output.Flags |= (byte)Flag.NEGATIVE;
        if((input.B & 0x40) != 0) output.Flags |= (byte)Flag.OVERFLOW;
        
        return output;
    }
}