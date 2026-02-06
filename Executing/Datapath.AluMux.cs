namespace mos6502.Executing;
using Computing;
using Signaling;

public partial class Datapath
{
    private readonly Alu Alu = new ();
    private readonly Sru Sru = new ();
    
    private byte flagLatch;

    private void AluCompute()
    {
        AluOutput output = Alu.Compute(new AluInput
        {
            A = Point(signal.First).Get(),
            B = Point(signal.Second).Get(),
            C = (byte)(Sru.Carry ? 1 : 0),
            F = flagLatch,
            DecimalMode = Sru.Decimal,
        }, signal.Operation);

        flagLatch = output.Flags;
        
        Point(Pointer.TMP).Set(output.Result);
        Point(Pointer.SR).Set((byte)
            ((Point(Pointer.SR).Get() & (byte)~signal.Mask) | (flagLatch & (byte)signal.Mask)));
    }
}