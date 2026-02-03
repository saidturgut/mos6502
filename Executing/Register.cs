namespace mos6502.Executing;

public class Register
{
    private byte value;

    public void Set(byte input)
        => value = input;
    
    public byte Get()
        => value;
}