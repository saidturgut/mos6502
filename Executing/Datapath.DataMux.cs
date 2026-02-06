namespace mos6502.Executing;
using Signaling;

public partial class Datapath
{
    private readonly Rom Rom = new();
    private readonly Ram Ram = new();

    private void RegisterWrite()
    {
        Point(signal.Second).Set(Point(signal.First).Get());
        //Console.WriteLine($"REG WRITE: {signal.Second} {Point(signal.Second).Get()}");
    }

    private void MemoryRead()
    {        
        Point(Pointer.TMP).Set(Ram.Read(Merge(Point(signal.First).Get(), Point(signal.Second).Get())));
        //Console.WriteLine($"RAM READ: {Hex(Merge(Point(signal.First).Get(), Point(signal.Second).Get()))} {Hex(Point(Pointer.TMP).Get())}");
    }

    private void MemoryWrite()
    {
        Ram.Write(Merge(
            Point(signal.First).Get(), Point(signal.Second).Get()), Point(Pointer.TMP).Get());
        if(debugMode) DebugMemoryWrite();
    }
    
    private void Increment()
    {
        Point(signal.First).Set((byte)(Point(signal.First).Get() + 1));
        if (Point(signal.First).Get() == 0x00 && signal.Second is not Pointer.NIL)
            Point(signal.Second).Set((byte)(Point(signal.Second).Get() + 1));
    }

    private void Decrement()
    {
        Point(signal.First).Set((byte)(Point(signal.First).Get() - 1));
        if (Point(signal.First).Get() == 0xFF && signal.Second is not Pointer.NIL)
            Point(signal.Second).Set((byte)(Point(signal.Second).Get() - 1));
    }
    
    private ushort Merge(byte low, byte high)
        => (ushort)(low + (high << 8));
    
    public void Clear()
    {
        debugName = "";
        Point(Pointer.NIL).Set(0);
        Point(Pointer.TMP).Set(0);
        Point(Pointer.WR).Set(0);
        Point(Pointer.ZR).Set(0);
        flagLatch = 0;
        stall = false;
    }
}