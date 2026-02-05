namespace mos6502.Executing.Computing;

public partial class Alu
{
    private static AluOutput NONE(AluInput input) => new();

    private static AluOutput ADD(AluInput input) => new()
        { Result = (byte)(input.A + input.B) };
    private static AluOutput SUB(AluInput input) => new()
        { Result = (byte)(input.A + ~input.B + 1) };
    
    private static AluOutput ADC(AluInput input)
    {
        var result = input.A + input.B + input.C;
        AluOutput output = new() { Result = (byte)result };

        if (Carry(result, 8)) output.Flags |= (byte)Flag.CARRY;
        if (SignedOverflow(input.A, input.B, output.Result)) output.Flags |= (byte)Flag.OVERFLOW;
        
        return output;
    }
    private static AluOutput SBC(AluInput input)
    {
        var result = input.A + ~input.B + input.C;
        AluOutput output = new() { Result = (byte)result };
        
        if (Carry(result, 8)) output.Flags |= (byte)Flag.CARRY;
        if (SignedOverflow(input.A, (byte)~input.B, output.Result)) output.Flags |= (byte)Flag.OVERFLOW;
        
        return output;
    }

    private static AluOutput AND(AluInput input) => new()
        { Result = (byte)(input.A & input.B) };
    private static AluOutput EOR(AluInput input) => new()
        { Result = (byte)(input.A ^ input.B) };
    private static AluOutput OR(AluInput input) => new()
        { Result = (byte)(input.A | input.B) };

    private static AluOutput INC(AluInput input)
    { input.B = 1; return ADD(input); }
    private static AluOutput DEC(AluInput input)
    { input.B = 1; return SUB(input); }

    private static AluOutput CLR(AluInput input) => new()
        { Flags = 0x00 };
    private static AluOutput SET(AluInput input) => new()
        { Flags = 0xFF };

    private static bool Carry(int source, byte bit)
        => (byte)((source >> bit) & 1) != 0;
    private static bool SignedOverflow(byte A, byte B, byte result)
        => (~(A ^ B) & (A ^ result) & 0x80) != 0;
}