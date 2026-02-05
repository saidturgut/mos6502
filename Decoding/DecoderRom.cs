namespace mos6502.Decoding;
using Executing.Computing;
using Microcodes;

public class DecoderRom : Microcode
{
    protected static readonly Signal[][] Table = new Signal[256][];

    protected static readonly Dictionary<string, Func<Signal[]>> MnemonicTable = new()
    {
        ["NOP"] = () => [CHANGE_STATE(Cycle.IDLE)],

        // LOAD & STORE
        ["LDA"] = () => LOAD(Pointer.A), ["LDX"] = () => LOAD(Pointer.X), ["LDY"] = () => LOAD(Pointer.Y),
        ["STA"] = () => STORE(Pointer.A), ["STX"] = () => STORE(Pointer.X), ["STY"] = () => STORE(Pointer.Y),

        // TRANSFER
        ["TAX"] = () => TRANSFER(Pointer.A, Pointer.X, true), ["TAY"] = () => TRANSFER(Pointer.A, Pointer.Y, true),
        ["TXA"] = () => TRANSFER(Pointer.X, Pointer.A, true), ["TYA"] = () => TRANSFER(Pointer.Y, Pointer.A, true),
        ["TXS"] = () => TRANSFER(Pointer.X, Pointer.SP, true), ["TSX"] = () => TRANSFER(Pointer.SP, Pointer.X, false),

        // ----------------------------------------------------------------------------------------------------------------------- //

        // INC & DEC
        ["INC"] = () => ALU_MEM(Operation.INC, FlagMask.ZN), ["DEC"] = () => ALU_MEM(Operation.DEC, FlagMask.ZN),
        ["INX"] = () => ALU_REG(Operation.INC, Pointer.X, FlagMask.ZN), ["DEX"] = () => ALU_REG(Operation.DEC, Pointer.X, FlagMask.ZN),
        ["INY"] = () => ALU_REG(Operation.INC, Pointer.Y, FlagMask.ZN), ["DEY"] = () => ALU_REG(Operation.DEC, Pointer.Y, FlagMask.ZN),
        
        // ARITHMETIC & LOGIC
        ["ADC"] = () => ALU(Operation.ADC, Pointer.A, FlagMask.CZVN, true), ["SBC"] = () => ALU(Operation.SBC, Pointer.A, FlagMask.CZVN, true),
        ["AND"] = () => ALU(Operation.AND, Pointer.A, FlagMask.ZN, true), ["CMP"] = () => ALU(Operation.SBC, Pointer.A, FlagMask.CZN, false),
        ["ORA"] = () => ALU(Operation.OR, Pointer.A, FlagMask.ZN, true), ["CPX"] = () => ALU(Operation.SBC, Pointer.X, FlagMask.CZN, false),
        ["EOR"] = () => ALU(Operation.EOR, Pointer.A, FlagMask.ZN, true), ["CPY"] = () => ALU(Operation.SBC, Pointer.Y, FlagMask.CZN, false),
        
        // SHIFT & ROTATE
        ["BIT"] = () => ALU(Operation.BIT, Pointer.A, FlagMask.ZVN, false),
        ["ASL"] = () => ALU_MEM(Operation.ASL, FlagMask.CZN), ["ASLA"] = () => ALU_REG(Operation.ASL, Pointer.A, FlagMask.CZN), 
        ["LSR"] = () => ALU_MEM(Operation.LSR, FlagMask.CZN), ["LSRA"] = () => ALU_REG(Operation.LSR, Pointer.A, FlagMask.CZN),
        ["ROL"] = () => ALU_MEM(Operation.ROL, FlagMask.CZN), ["ROLA"] = () => ALU_REG(Operation.ROL, Pointer.A, FlagMask.CZN), 
        ["ROR"] = () => ALU_MEM(Operation.ROR, FlagMask.CZN), ["RORA"] = () => ALU_REG(Operation.ROR, Pointer.A, FlagMask.CZN),

        // FLAG CLEAR & SET
        ["SEC"] = () => CLR_SET(false, Flag.CARRY), ["SED"] = () => CLR_SET(false, Flag.DECIMAL), ["SEI"] = () => CLR_SET(false, Flag.INTERRUPT),
        ["CLC"] = () => CLR_SET(true, Flag.CARRY), ["CLD"] = () => CLR_SET(true, Flag.DECIMAL), ["CLI"] = () => CLR_SET(true, Flag.INTERRUPT), 
        ["CLV"] = () => CLR_SET(true, Flag.OVERFLOW),
        
        // ----------------------------------------------------------------------------------------------------------------------- //

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
        ["IMP"] = () => IMPLIED,
        ["IMM"] = () => IMMEDIATE,
        ["ZP"] = () => ZERO_PAGE(Pointer.NIL),
        ["ZPX"] = () => ZERO_PAGE(Pointer.X),
        ["ZPY"] = () => ZERO_PAGE(Pointer.Y),
        ["ABS"] = () => ABSOLUTE(Pointer.NIL),
        ["ABSX"] = () => ABSOLUTE(Pointer.X),
        ["ABSY"] = () => ABSOLUTE(Pointer.Y),
        ["IND"] = () => INDIRECT,
        ["INDX"] = () => INDIRECT_X,
        ["INDY"] = () => INDIRECT_Y,
        ["REL"] = () => RELATIVE,
    };
}