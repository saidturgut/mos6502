namespace mos6502.Executing.Computing;

// ARITHMETIC LOGIC UNIT
public partial class Alu
{
    public AluOutput Compute(AluInput input, Operation operation)
    {
        AluOutput output = Operations[(byte)operation](input);
        
        if (output.Custom) return output;
        
        if ((output.Result & 0x80) != 0) output.Flags |= (byte)Flag.NEGATIVE;
        if (output.Result == 0) output.Flags |= (byte)Flag.ZERO;

        return output;
    }
    
    private static readonly Func<AluInput, AluOutput>[] Operations =
    [
        NONE, 
        ADD, ADC, DAD, SBC, DSB,
        AND, OR, EOR, INC, DEC,
        ASL, LSR, ROL, ROR, BIT,
        CLR, SET, CRY, PSR, SRP,
    ];
}

public enum Operation
{
    NONE, 
    ADD, ADC, DAD, SBC, DSB,
    AND, OR, EOR, INC, DEC,
    ASL, LSR, ROL, ROR, BIT,
    CLR, SET, CRY, PSR, SRP,
}

public struct AluInput
{
    public byte A;
    public byte B;
    public byte C;
    public byte F;
    public bool DecimalMode;
}

public struct AluOutput
{
    public byte Result;
    public byte Flags;
    public bool Custom;
}