namespace mos6502.Executing;
using Signaling;

public partial class Datapath
{
    private readonly Register[] Registers = new Register[13];

    private Signal signal = new();

    private bool debugMode;
    private bool stall;
    
    public void Init(bool debug)
    {
        debugMode = debug;
        for (int i = 0; i < Registers.Length; i++)
            Registers[i] =  new Register();

        Rom.Boot(Ram);
        
        //Point(Pointer.SP).Set(0xFD);
        //Point(Pointer.SR).Set(0x20);
        //Point(Pointer.PCH).Set(0x80);
        //Point(Pointer.IX).Set(0x7);
        //Point(Pointer.IY).Set(0x7);
    }
    
    public void Receive(Signal input)
        => signal = input;

    public void Execute()
    {
        switch (signal.Cycle)
        {
            case Cycle.REG_COMMIT: RegisterWrite(); break;
            case Cycle.MEM_READ: MemoryRead(); break;
            case Cycle.MEM_WRITE: MemoryWrite(); break;
            case Cycle.ALU_COMPUTE: AluCompute(); break;
            case Cycle.PAIR_INC: Increment(); break;
            case Cycle.PAIR_DEC: Decrement(); break;
            default: stall = !Sru.Check(signal.Condition); break;
        }
        Protocol();
    }

    private void Protocol()
    {
        if (signal.Name != "") debugName = signal.Name;
        Sru.Update(Point(Pointer.SR).Get());
        Point(Pointer.NIL).Set(0);
    }

    private Register Point(Pointer pointer)
        => Registers[(byte)pointer];

    public ControlSignal Emit()
        => new(Point(Pointer.IR).Get(), stall);
}

public struct ControlSignal(byte opcode, bool stall)
{
    public byte Opcode = opcode;
    public bool Stall = stall;
}