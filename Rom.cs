namespace mos6502;

public class Rom
{
    public void Boot(Ram ram)
    {
        ram.LoadImage(0, File.ReadAllBytes("test.bin"));
        /*ram.LoadByte(0x8, 0xBB);
        ram.LoadByte(0x30, 0xFF);
        ram.LoadByte(0x31, 0x12);
        ram.LoadByte(0x8000, 0x77);
        ram.LoadByte(0x20, 0x40);
        ram.LoadByte(0x21, 0x80);
        ram.LoadByte(0xFF, 0x87);
        ram.LoadByte(0x12FF, 0x00);
        ram.LoadByte(0x1200, 0x80);
        ram.LoadByte(0x1238, 0x66);
        ram.LoadByte(0x2000, 0x77);
        ram.LoadByte(0x2008, 0x88);*/
        //ram.MemoryDump();
    }
}