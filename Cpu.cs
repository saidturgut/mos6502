namespace mos6502;
using Signaling;
using Executing;

public class Cpu
{
    private readonly Datapath Datapath = new();
    private readonly Control Control = new();

    private const bool debugMode = false;
    
    public void Power() => Clock();

    private void Clock()
    {
        Datapath.Init(debugMode);
        Control.Init();
        
        while (!Control.halt) Tick();;
    }

    private void Tick()
    {
        Datapath.Receive(Control.Emit());
        
        Datapath.Execute();

        Control.Advance(Datapath.Emit());

        if (Control.commit) Commit();
    }

    private void Commit()
    {
        Datapath.Debug();
        Datapath.Clear();
    }
}