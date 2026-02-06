namespace mos6502;

public class Rom
{
    public void Boot(Ram ram)
    {
        ram.LoadImage(0x8000, File.ReadAllBytes("test.bin"));
        ram.LoadByte(0x10, 0x11);
        ram.LoadByte(0x14, 0x22);
        ram.LoadByte(0x18, 0x33);
        ram.LoadByte(0x20, 0x40);
        ram.LoadByte(0x21, 0x80);
        ram.LoadByte(0x1234, 0x55);
        ram.LoadByte(0x1238, 0x66);
        ram.LoadByte(0x2000, 0x77);
        ram.LoadByte(0x2008, 0x88);
        //ram.MemoryDump();
    }
}