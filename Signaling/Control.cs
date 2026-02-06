namespace mos6502.Signaling;
using Executing;

public class Control
{
    private readonly Decoder Decoder = new();
    private Signal[] decoded = [];

    private byte timeState;
    
    public bool commit;
    public bool halt;

    public void Init()
    {
        decoded = Decoder.Fetch;
    }

    public Signal Emit()
        => decoded[timeState];

    public void Advance(ControlSignal signal)
    {
        if (timeState != decoded.Length - 1 && !signal.Stall)
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
                
                //Console.WriteLine($"TOTAL CYCLES : {decoded.Length}");
                
                if (decoded == Array.Empty<Signal>())
                {
                    throw new Exception($"ILLEGAL OPCODE \"{signal.Opcode}\"");
                }
                break;
            }
            default: decoded = Decoder.Fetch; commit = true; break;
        }
        timeState = 0;
    }

    public void Clear()
    {
        commit = false;
    }
}
