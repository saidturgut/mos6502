namespace mos6502;

public class Tty
{
    public Device Device = new()
    {
        Min = 0xC000,
        Max = 0xC0FF,
    };

    public void Init()
    {
        Device.Read = Read;
        Device.Write = Write;
    }

    public byte Read(ushort address)
    {
        throw new Exception("TTY READ STUB");
    }

    public void Write(ushort address, byte data)
    {
        throw new Exception("TTY WRITE STUB");
    }
}