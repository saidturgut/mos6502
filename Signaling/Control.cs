namespace mos6502.Signaling;
using Executing;

public class Control
{
    private readonly Decoder Decoder = new();
    private Signal[] decoded = [];

    private byte timeState;
    
    public bool commit;
    public bool halt;
    
    public void Init(){}

    public Signal Emit()
        => decoded[timeState];

    public void Advance(ControlSignal signal)
    {
        if (timeState < decoded.Length && !signal.Stall)
            timeState++;
        else
            Commit(signal);
    }

    private void Commit(ControlSignal signal)
    {
        switch (decoded[timeState].Cycle)
        {
            case Cycle.HALT: halt = true; break;
            case Cycle.DECODE:
            {
                decoded = Decoder.Decode(signal.Opcode);
                    
                if (decoded == Array.Empty<Signal>())
                {
                    throw new Exception($"ILLEGAL OPCODE \"{signal.Opcode}\"");
                }
                break;
            }
            default: decoded = Decoder.Fetch(); commit = true; break;
        }
            
        timeState = 0;
    }
}
