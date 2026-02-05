namespace mos6502.Executing.Computing;

// STATUS REGISTER UNIT
public class Sru
{
    public bool Carry;
    public bool Zero;
    public bool Interrupt;
    public bool Decimal;
    public bool Break;
    public bool Unused;
    public bool Overflow;
    public bool Negative;

    public void Update(byte sr)
    {
        Carry = (byte)(sr & (byte)Flag.CARRY) != 0;
        Zero = (byte)(sr & (byte)Flag.ZERO) != 0;
        Interrupt = (byte)(sr & (byte)Flag.INTERRUPT) != 0;
        Decimal = (byte)(sr & (byte)Flag.DECIMAL) != 0;
        Break = (byte)(sr & (byte)Flag.BREAK) != 0;
        Unused = (byte)(sr & (byte)Flag.UNUSED) != 0;
        Overflow = (byte)(sr & (byte)Flag.OVERFLOW) != 0;
        Negative = (byte)(sr & (byte)Flag.NEGATIVE) != 0;
    }
    
    public bool Check(Condition condition) => condition switch
    {
        Condition.CC => !Carry,
        Condition.CS => Carry,
        Condition.NE => !Zero,
        Condition.EQ => Zero,
        Condition.VC => !Overflow,
        Condition.VS => Overflow,
        Condition.PL => !Negative,
        Condition.MI => Negative,
        _ => false
    };
}

[Flags]
public enum Flag
{
    NONE = -1,
    CARRY = 1 << 0,
    ZERO = 1 << 1,
    INTERRUPT = 1 << 2,
    DECIMAL = 1 << 3,
    BREAK = 1 << 4,
    UNUSED = 1 << 5,
    OVERFLOW = 1 << 6,
    NEGATIVE = 1 << 7,
}

public enum Condition
{       // CARRY, ZERO, OVERFLOW, NEGATIVE
    NONE, CC, CS, NE, EQ, VC, VS, PL, MI,
}