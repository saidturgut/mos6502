namespace mos6502.Signaling;
using Decoding;

public class Control
{
    private Decoder Decoder = new();
    private Signal[] Decoded = [];

    private byte timeState;
    
    public bool commit;
    public bool halt;
    
    public void Init(){}

    public void Emit()
    {
        
    }

    public void Advance(byte opcode)
    {
        if (timeState < Decoded.Length)
        {
            timeState++;
        }
        else
        {
            switch (Decoded[timeState].Cycle)
            {
                case Cycle.HALT: break;
                case Cycle.DECODE:
                {
                    Decoded = Decoder.Decode(opcode);
                    
                    if (Decoded == Array.Empty<Signal>())
                    {
                        throw new Exception($"ILLEGAL OPCODE \"{opcode}\"");
                    }
                    break;
                }
                default: Decoded = Decoder.Fetch(); commit = true; break;
            }
            
            timeState = 0;
        }
    }
}
