namespace mos6502.Executing;
using Decoding;

public partial class Datapath
{
    private readonly Register[] Registers = new Register[12];

    private Signal signal = new();

    private bool stall;
    
    public void Init()
    {
        for (int i = 0; i < Registers.Length; i++)
            Registers[i] =  new Register();
    }

    private Register Point(Pointer pointer)
        => Registers[(byte)pointer];

    public byte Opcode()
        => Point(Pointer.IR).Get();
}
