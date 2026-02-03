namespace mos6502.Signaling;
using Decoding;

public class Control
{
    private Decoder Decoder = new();
    private Signal[] Decoded = [];

    private byte timeState;
    
    public bool COMMIT;
    public bool HALT;
    
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
                case Cycle.DECODE: Decoded = Decoder.Decode(opcode); break;
                default: Decoded = Decoder.FETCH; COMMIT = true; break;
            }
            
            timeState = 0;
        }
    }
}
