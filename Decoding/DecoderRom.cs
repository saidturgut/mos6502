namespace mos6502.Decoding;
using Executing.Computing;
using Microcodes;

public class DecoderRom : Microcode
{
    protected static readonly Signal[][] Table = new Signal[256][];

    protected static readonly Dictionary<string, Func<Signal[]>> MnemonicTable = new()
    {
        ["NOP"] = () => [CHANGE_STATE(Cycle.IDLE)],
        ["HLT"] = () => [CHANGE_STATE(Cycle.HALT)],

        // LOAD & STORE
        ["LDA"] = () => LOAD(Pointer.ACC), ["LDX"] = () => LOAD(Pointer.IX), ["LDY"] = () => LOAD(Pointer.IY),
        ["STA"] = () => STORE(Pointer.ACC), ["STX"] = () => STORE(Pointer.IX), ["STY"] = () => STORE(Pointer.IY),

        // TRANSFER
        ["TAX"] = () => TRANSFER(Pointer.ACC, Pointer.IX, true), ["TAY"] = () => TRANSFER(Pointer.ACC, Pointer.IY, true),
        ["TXA"] = () => TRANSFER(Pointer.IX, Pointer.ACC, true), ["TYA"] = () => TRANSFER(Pointer.IY, Pointer.ACC, true),
        ["TXS"] = () => TRANSFER(Pointer.IX, Pointer.SP, true), ["TSX"] = () => TRANSFER(Pointer.SP, Pointer.IX, false),

        // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        // INC & DEC
        ["INC"] = () => ALU_MEM(Operation.INC, FlagMask.ZN), ["DEC"] = () => ALU_MEM(Operation.DEC, FlagMask.ZN),
        ["INX"] = () => ALU_REG(Operation.INC, Pointer.IX, FlagMask.ZN), ["DEX"] = () => ALU_REG(Operation.DEC, Pointer.IX, FlagMask.ZN),
        ["INY"] = () => ALU_REG(Operation.INC, Pointer.IY, FlagMask.ZN), ["DEY"] = () => ALU_REG(Operation.DEC, Pointer.IY, FlagMask.ZN),
        
        // ARITHMETIC & LOGIC
        ["ADC"] = () => ALU(Operation.ADC, Pointer.ACC, FlagMask.CZVN, true), ["SBC"] = () => ALU(Operation.SBC, Pointer.ACC, FlagMask.CZVN, true),
        ["AND"] = () => ALU(Operation.AND, Pointer.ACC, FlagMask.ZN, true), ["CMP"] = () => ALU(Operation.SBC, Pointer.ACC, FlagMask.CZN, false),
        ["ORA"] = () => ALU(Operation.OR, Pointer.ACC, FlagMask.ZN, true), ["CPX"] = () => ALU(Operation.SBC, Pointer.IX, FlagMask.CZN, false),
        ["EOR"] = () => ALU(Operation.EOR, Pointer.ACC, FlagMask.ZN, true), ["CPY"] = () => ALU(Operation.SBC, Pointer.IY, FlagMask.CZN, false),
        
        // SHIFT & ROTATE
        ["BIT"] = () => ALU(Operation.BIT, Pointer.ACC, FlagMask.ZVN, false),
        ["ASL"] = () => ALU_MEM(Operation.ASL, FlagMask.CZN), ["ASLA"] = () => ALU_REG(Operation.ASL, Pointer.ACC, FlagMask.CZN), 
        ["LSR"] = () => ALU_MEM(Operation.LSR, FlagMask.CZN), ["LSRA"] = () => ALU_REG(Operation.LSR, Pointer.ACC, FlagMask.CZN),
        ["ROL"] = () => ALU_MEM(Operation.ROL, FlagMask.CZN), ["ROLA"] = () => ALU_REG(Operation.ROL, Pointer.ACC, FlagMask.CZN), 
        ["ROR"] = () => ALU_MEM(Operation.ROR, FlagMask.CZN), ["RORA"] = () => ALU_REG(Operation.ROR, Pointer.ACC, FlagMask.CZN),

        // FLAG CLEAR & SET
        ["SEC"] = () => CLR_SET(false, Flag.CARRY), ["SED"] = () => CLR_SET(false, Flag.DECIMAL), ["SEI"] = () => CLR_SET(false, Flag.INTERRUPT),
        ["CLC"] = () => CLR_SET(true, Flag.CARRY), ["CLD"] = () => CLR_SET(true, Flag.DECIMAL), ["CLI"] = () => CLR_SET(true, Flag.INTERRUPT), 
        ["CLV"] = () => CLR_SET(true, Flag.OVERFLOW),
        
        // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

        // CONTROL FLOW
        ["JMP"] = () => NONE, ["JSR"] = () => NONE,
        ["RTI"] = () => NONE, ["RTS"] = () => NONE,

        // PUSH & PULL
        ["PHA"] = () => NONE, ["PHP"] = () => NONE,
        ["PLA"] = () => NONE, ["PLP"] = () => NONE,

        // BRANCH CLEAR & SET
        ["BCC"] = () => BRANCH(Condition.CC), ["BCS"] = () => BRANCH(Condition.CS),
        ["BNE"] = () => BRANCH(Condition.NE), ["BEQ"] = () => BRANCH(Condition.EQ),
        ["BPL"] = () => BRANCH(Condition.PL), ["BMI"] = () => BRANCH(Condition.MI),
        ["BVC"] = () => BRANCH(Condition.VC), ["BVS"] = () => BRANCH(Condition.VS),
        ["BRK"] = () => NONE,
    };
    
    protected static readonly Dictionary<string, Func<Signal[]>> AddressingTable = new()
    {
        ["IMP"] = () => IMPLIED,
        ["IMM"] = () => IMMEDIATE,
        ["ZP"] = () => ZERO_PAGE(Pointer.NIL),
        ["ZPX"] = () => ZERO_PAGE(Pointer.IX),
        ["ZPY"] = () => ZERO_PAGE(Pointer.IY),
        ["ABS"] = () => ABSOLUTE(Pointer.NIL),
        ["ABSX"] = () => ABSOLUTE(Pointer.IX),
        ["ABSY"] = () => ABSOLUTE(Pointer.IY),
        ["IND"] = () => INDIRECT,
        ["INDX"] = () => INDIRECT_X,
        ["INDY"] = () => INDIRECT_Y,
    };
}