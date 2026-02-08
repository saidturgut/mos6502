namespace mos6502;
using Testing;

public class Ram
{
    private readonly byte[] Memory = new byte[0x7FFF];
    
    public Device Device = new()
    {
        Min = 0x0000,
        Max = 0x7FFF,
    };

    public void Init()
    {
        Device.Read = Read;
        Device.Write = Write;
    }
    
    public void MemoryDump() => HexDump.Run(Memory);

    public void LoadByte(ushort address, byte data)
        => Memory[address] = data;
    
    public void LoadImage(ushort address, byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
            Memory[address + i] = data[i];
    }

    public byte Read(ushort address)
        => Memory[address];
    
    public void Write(ushort address, byte data)
        => Memory[address] = data;
}