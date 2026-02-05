namespace mos6502.Executing;
using Computing;
using Decoding;

public partial class Datapath
{
    private readonly Alu Alu = new ();
    private readonly Sru Sru = new ();
    
    private byte flagLatch; // CLEARED ON BOUNDARY, WRITTEN PER OPERATION

    private void AluCompute()
    {
        AluOutput output = Alu.Compute(new AluInput
        {
            A = Point(signal.First).Get(),
            B = Point(signal.Second).Get(),
            C = (byte)(Sru.Carry ? 1 : 0),
            F = flagLatch,
        }, signal.Operation);

        flagLatch = output.Flags;
        
        Point(Pointer.TMP).Set(output.Result);
        Point(Pointer.SR).Set((byte)
            ((Point(Pointer.SR).Get() & (byte)~signal.Mask) | (flagLatch & (byte)signal.Mask)));
    }
}