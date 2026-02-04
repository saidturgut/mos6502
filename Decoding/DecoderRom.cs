namespace mos6502.Decoding;
using Microcodes;

public class DecoderRom : Microcode
{
    protected static readonly Signal[][] Table = new Signal[256][];

    protected static readonly Dictionary<string, Func<Signal[]>> MnemonicTable = new()
    {
        ["NOP"] = () => NONE,

        // LOAD & STORE
        ["LDA"] = () => LD(Pointer.A), ["LDX"] = () => LD(Pointer.X), ["LDY"] = () => LD(Pointer.Y),
        ["STA"] = () => NONE, ["STX"] = () => NONE, ["STY"] = () => NONE,

        // TRANSFER
        ["TAX"] = () => NONE, ["TAY"] = () => NONE,
        ["TXA"] = () => NONE, ["TYA"] = () => NONE,
        ["TXS"] = () => NONE, ["TSX"] = () => NONE,

// --------------------------------------------------------------------- //

        // INC & DEC
        ["INC"] = () => NONE, ["INX"] = () => NONE, ["INY"] = () => NONE,
        ["DEC"] = () => NONE, ["DEX"] = () => NONE, ["DEY"] = () => NONE,

        // ARITHMETIC & LOGIC
        ["ADC"] = () => NONE, ["SBC"] = () => NONE,
        ["CMP"] = () => NONE, ["CPX"] = () => NONE, ["CPY"] = () => NONE,
        ["AND"] = () => NONE, ["ORA"] = () => NONE, ["EOR"] = () => NONE,

        // BITWISE
        ["BIT"] = () => NONE,
        ["ASL"] = () => NONE, ["LSR"] = () => NONE,
        ["ROL"] = () => NONE, ["ROR"] = () => NONE,

        // FLAG CLEAR & SET
        ["CLC"] = () => NONE, ["SEC"] = () => NONE,
        ["CLD"] = () => NONE, ["SED"] = () => NONE,
        ["CLI"] = () => NONE, ["SEI"] = () => NONE,
        ["CLV"] = () => NONE,

// --------------------------------------------------------------------- //

        // CONTROL FLOW
        ["JMP"] = () => NONE, ["JSR"] = () => NONE,
        ["RTI"] = () => NONE, ["RTS"] = () => NONE,

        // PUSH & PULL
        ["PHA"] = () => NONE, ["PHP"] = () => NONE,
        ["PLA"] = () => NONE, ["PLP"] = () => NONE,

        // BRANCH CLEAR & SET
        ["BCC"] = () => NONE, ["BCS"] = () => NONE,
        ["BNE"] = () => NONE, ["BEQ"] = () => NONE,
        ["BPL"] = () => NONE, ["BMI"] = () => NONE,
        ["BVC"] = () => NONE, ["BVS"] = () => NONE,
        ["BRK"] = () => NONE,
    };
    
    protected static readonly Dictionary<string, Func<Signal[]>> AddressingTable = new()
    {
        ["IMP"] = () => IMPLICIT,
        ["ACC"] = () => ACCUMULATOR,
        ["IMM"] = () => IMMEDIATE,
        ["ZP"] = () => ZERO_PAGE(Pointer.ZERO),
        ["ZPX"] = () => ZERO_PAGE(Pointer.X),
        ["ZPY"] = () => ZERO_PAGE(Pointer.Y),
        ["ABS"] = () => ABSOLUTE(Pointer.ZERO),
        ["ABSX"] = () => ABSOLUTE(Pointer.X),
        ["ABSY"] = () => ABSOLUTE(Pointer.Y),
        ["IND"] = () => INDIRECT(Pointer.ZERO),
        ["INDX"] = () => INDIRECT(Pointer.X),
        ["INDY"] = () => INDIRECT(Pointer.Y),
        ["REL"] = () => RELATIVE,
    };
}