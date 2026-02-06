namespace mos6502.Executing.Computing;

public partial class Alu
{
    private static AluOutput DAD(AluInput input)
    {
        var result = input.A + input.B + input.C;
        var fixer = 0;

        if ((result & 0x0F) > 9) fixer |= 0x06;
        if (result > 0x99) fixer |= 0x60;

        result += fixer;

        AluOutput output = new() { Result = (byte)result };

        if (result > 0xFF) output.Flags |= (byte)Flag.CARRY;

        return output;
    }
    
    private static AluOutput DSB(AluInput input)
    {
        var result = input.A - input.B - (1 - input.C);
        var fixer = 0;

        if ((input.A & 0x0F) - (input.B & 0x0F) - (1 - input.C) < 0) fixer |= 0x06;

        if (result < 0) fixer |= 0x60;

        result -= fixer;

        AluOutput output = new() { Result = (byte)result };

        if (result >= 0) output.Flags |= (byte)Flag.CARRY;

        return output;
    }
    
    private static AluOutput CRY(AluInput input) => new()
        { Result = (byte)(input.A + (input.F & (byte)Flag.CARRY)) };
    
    private static AluOutput PSR(AluInput input) => new()
        { Result = (byte)(input.A | (byte)Flag.BREAK | (byte)Flag.UNUSED) };
    private static AluOutput SRP(AluInput input) => new()
        { Result = (byte)(input.A & ~((byte)Flag.BREAK | (byte)Flag.UNUSED)) };

    private static AluOutput CLR(AluInput input) => new()
        { Flags = 0x00 };
    private static AluOutput SET(AluInput input) => new()
        { Flags = 0xFF };
}