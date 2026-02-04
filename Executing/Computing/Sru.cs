namespace mos6502.Executing.Computing;

// STATUS REGISTER UNIT
public class Sru
{
    private readonly bool[] Flags = new bool[8];

    public bool Get(Flag flag)
        => Flags[(byte)flag];
    
    public void Set(Flag flag, bool value)
        => Flags[(byte)flag] = value;

    public byte GetByte()
    {
        byte word = 0;
        if (Flags[0]) word |= 1 << 0;
        if (Flags[1]) word |= 1 << 1;
        if (Flags[2]) word |= 1 << 2;
        if (Flags[3]) word |= 1 << 3;
        if (Flags[4]) word |= 1 << 4;
        if (Flags[5]) word |= 1 << 5;
        if (Flags[6]) word |= 1 << 6;
        if (Flags[7]) word |= 1 << 7;
        return word;
    }
}

[Flags]
public enum Flag
{
    NONE = -1,
    CARRY, ZERO, INTERRUPT, DECIMAL,
    BREAK, UNUSED, OVERFLOW, NEGATIVE,
}