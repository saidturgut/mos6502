namespace mos6502;
using Signaling;
using Executing;

public class Cpu
{
    private readonly Datapath Datapath = new();
    private readonly Control Control = new();
    
    public void Power() => Clock();

    private void Clock()
    {
        Datapath.Init();
        Control.Init();
        
        while (!Control.HALT) Tick();;
    }

    private void Tick()
    {
        Control.Emit();
        
        // EXECUTE
        
        Control.Advance(Datapath.Opcode());
    }
}