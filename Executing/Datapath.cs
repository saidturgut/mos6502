namespace mos6502.Executing;
using Signaling;

public partial class Datapath
{
    private readonly Register[] Registers = new Register[12];

    private Signal signal = new();

    private bool debugMode;
    private bool stall;
    
    public void Init(bool debug)
    {
        debugMode = debug;
        for (int i = 0; i < Registers.Length; i++)
            Registers[i] =  new Register();
    }
    
    public void Receive(Signal input)
        => signal = input;

    public void Execute()
    {
        switch (signal.Cycle)
        {
            case Cycle.REG_WRITE: RegisterWrite(); break;
            case Cycle.MEM_READ: MemoryRead(); break;
            case Cycle.MEM_WRITE: MemoryWrite(); break;
            case Cycle.ALU_COMPUTE: AluCompute(); break;
            case Cycle.PAIR_INC: Increment(); break;
            case Cycle.PAIR_DEC: Decrement(); break;
            default: stall = !Sru.Check(signal.Condition); break;
        }
        Sru.Update(Point(Pointer.SR).Get());
    }

    private Register Point(Pointer pointer)
        => Registers[(byte)pointer];

    public ControlSignal Emit()
        => new(Point(Pointer.IR).Get(), stall);
}

public struct ControlSignal(byte opcode, bool stall)
{
    public byte Opcode;
    public bool Stall;
}