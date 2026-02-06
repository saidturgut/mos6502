namespace mos6502.Executing;
using Signaling;

public partial class Datapath
{
    private string debugName;
    
    public void Debug()
    {
        if(!debugMode) return;
        
        Console.WriteLine($"-> {debugName}");
        
        ushort flags = DebugGet(Pointer.SR);
        Console.WriteLine($"IR: {Hex(DebugGet(Pointer.IR))}");

        Console.WriteLine($"PC: {Hex(Merge(DebugGet(Pointer.PCL), DebugGet(Pointer.PCH)))}");
        Console.WriteLine($"SP: {Hex(DebugGet(Pointer.SP))}");
        Console.WriteLine($"WZ: {Hex(Merge(DebugGet(Pointer.WR), DebugGet(Pointer.ZR)))}");
        
        Console.WriteLine($"A: {Hex(DebugGet(Pointer.ACC))}");
        Console.WriteLine($"X: {Hex(DebugGet(Pointer.IX))}");
        Console.WriteLine($"Y: {Hex(DebugGet(Pointer.IY))}");
        Console.WriteLine($"TMP: {Hex(DebugGet(Pointer.TMP))}");
        
        Console.WriteLine($"CZIDBVN");
        Console.WriteLine($"{(flags >> 0) & 1}{(flags >> 1) & 1}{(flags >> 2) & 1}{(flags >> 3) & 1}{(flags >> 4) & 1}{(flags >> 6) & 1}{(flags >> 7) & 1}");
        
        Console.WriteLine("-----------");
    }

    private void DebugMemoryWrite()
    {
        Console.WriteLine($"MEM[{Merge(Point(signal.First).Get(), Point(signal.Second).Get())}]: {Point(Pointer.TMP).Get()}");
    }
    
    private byte DebugGet(Pointer pointer)
        => Point(pointer).Get();
    
    private string Hex(ushort input)         
        => $"0x{Convert.ToString(input, 16).ToUpper()}";
}