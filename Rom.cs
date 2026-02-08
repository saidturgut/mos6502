namespace mos6502;

public class Rom
{
    private readonly byte[] Memory = File.ReadAllBytes("test.bin");

    public Device Device = new()
    {
        Min = 0x8000,
        Max = 0xBFFF,
    };

    public void Init()
    {
        Device.Read = Read;
        Device.Write = Write;
    }
    
    private byte Read(ushort address)
        => Memory[address];
    
    private void Write(ushort address, byte data)
        => throw new Exception("READ ONLY MEMORY!!");
}