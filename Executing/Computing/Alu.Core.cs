namespace mos6502.Executing.Computing;

public partial class Alu
{
    private static AluOutput NONE(AluInput input) => new()
        { Result = input.A, };
    
    private static AluOutput ADC(AluInput input)
    {
        if (input.DecimalMode) return DAD(input);
        
        var result = input.A + input.B + input.C;
        AluOutput output = new() { Result = (byte)result };

        if (Carry(result, 8)) output.Flags |= (byte)Flag.CARRY;
        if (SignedOverflow(input.A, input.B, output.Result)) output.Flags |= (byte)Flag.OVERFLOW;
        
        return output;
    }
    private static AluOutput SBC(AluInput input)
    {
        if (input.DecimalMode) return DSB(input);

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
    
    private static AluOutput INC(AluInput input) => new()
        { Result = (byte)(input.A + input.B), };
    private static AluOutput DEC(AluInput input) => new()
        { Result = (byte)(input.A + ~input.B + 1), };
    
    private static bool Carry(int source, byte bit)
        => (byte)((source >> bit) & 1) != 0;
    private static bool SignedOverflow(byte A, byte B, byte result)
        => (~(A ^ B) & (A ^ result) & 0x80) != 0;
}