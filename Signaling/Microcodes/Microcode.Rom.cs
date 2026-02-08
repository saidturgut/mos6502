namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    private static readonly Dictionary<string, Func<Signal[]>> AddressingTable = new()
    {
        ["-"] = () => [],
        ["IMP"] = () => IMPLIED,
        ["IMM"] = () => IMMEDIATE,
        ["ZP"] = () => ZERO_PAGE,
        ["ZPX"] = () => ZERO_PAGE_IDX(Pointer.IX),
        ["ZPY"] = () => ZERO_PAGE_IDX(Pointer.IY),
        ["ABS"] = () => ABSOLUTE,
        ["ABSX"] = () => ABSOLUTE_IDX(Pointer.IX),
        ["ABSY"] = () => ABSOLUTE_IDX(Pointer.IY),
        ["IND"] = () => INDIRECT,
        ["INDX"] = () => INDIRECT_X,
        ["INDY"] = () => INDIRECT_Y,
    };
    
    private static readonly Dictionary<string, Func<Signal[]>> MnemonicTable = new()
    {
        ["-"] = () => [CHANGE_STATE(Cycle.IDLE)],
        ["NOP"] = () => [CHANGE_STATE(Cycle.IDLE)],
        ["HLT"] = () => [CHANGE_STATE(Cycle.HALT)],

        // LOAD & STORE
        ["LDA"] = () => LOAD(Pointer.ACC), ["LDX"] = () => LOAD(Pointer.IX), ["LDY"] = () => LOAD(Pointer.IY),
        ["STA"] = () => STORE(Pointer.ACC), ["STX"] = () => STORE(Pointer.IX), ["STY"] = () => STORE(Pointer.IY),

        // TRANSFER
        ["TAX"] = () => TRANSFER(Pointer.ACC, Pointer.IX, true), ["TAY"] = () => TRANSFER(Pointer.ACC, Pointer.IY, true),
        ["TXA"] = () => TRANSFER(Pointer.IX, Pointer.ACC, true), ["TYA"] = () => TRANSFER(Pointer.IY, Pointer.ACC, true),
        ["TXS"] = () => TRANSFER(Pointer.IX, Pointer.SPL, true), ["TSX"] = () => TRANSFER(Pointer.SPL, Pointer.IX, false),

        //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-//

        // INC & DEC
        ["INC"] = () => ALU_MEM(Operation.INC, FlagMask.ZN), ["DEC"] = () => ALU_MEM(Operation.DEC, FlagMask.ZN),
        ["INX"] = () => ALU_REG(Operation.INC, Pointer.IX, FlagMask.ZN), ["DEX"] = () => ALU_REG(Operation.DEC, Pointer.IX, FlagMask.ZN),
        ["INY"] = () => ALU_REG(Operation.INC, Pointer.IY, FlagMask.ZN), ["DEY"] = () => ALU_REG(Operation.DEC, Pointer.IY, FlagMask.ZN),
        
        // ARITHMETIC & LOGIC
        ["ADC"] = () => ALU(Operation.ADC, Pointer.ACC, FlagMask.CZVN, true), ["SBC"] = () => ALU(Operation.SBC, Pointer.ACC, FlagMask.CZVN, true),
        ["AND"] = () => ALU(Operation.AND, Pointer.ACC, FlagMask.ZN, true), ["CMP"] = () => ALU(Operation.CMP, Pointer.ACC, FlagMask.CZN, false),
        ["ORA"] = () => ALU(Operation.OR, Pointer.ACC, FlagMask.ZN, true), ["CPX"] = () => ALU(Operation.CMP, Pointer.IX, FlagMask.CZN, false),
        ["EOR"] = () => ALU(Operation.EOR, Pointer.ACC, FlagMask.ZN, true), ["CPY"] = () => ALU(Operation.CMP, Pointer.IY, FlagMask.CZN, false),
        
        // SHIFT & ROTATE
        ["BIT"] = () => ALU(Operation.BIT, Pointer.ACC, FlagMask.ZVN, false),
        ["ASL"] = () => ALU_MEM(Operation.ASL, FlagMask.CZN), ["ASLA"] = () => ALU_REG(Operation.ASL, Pointer.ACC, FlagMask.CZN), 
        ["LSR"] = () => ALU_MEM(Operation.LSR, FlagMask.CZN), ["LSRA"] = () => ALU_REG(Operation.LSR, Pointer.ACC, FlagMask.CZN),
        ["ROL"] = () => ALU_MEM(Operation.ROL, FlagMask.CZN), ["ROLA"] = () => ALU_REG(Operation.ROL, Pointer.ACC, FlagMask.CZN), 
        ["ROR"] = () => ALU_MEM(Operation.ROR, FlagMask.CZN), ["RORA"] = () => ALU_REG(Operation.ROR, Pointer.ACC, FlagMask.CZN),

        // FLAG CLEAR & SET
        ["SEC"] = () => FLAG(false, Flag.CARRY), ["SED"] = () => FLAG(false, Flag.DECIMAL), ["SEI"] = () => FLAG(false, Flag.INTERRUPT),
        ["CLC"] = () => FLAG(true, Flag.CARRY), ["CLD"] = () => FLAG(true, Flag.DECIMAL), ["CLI"] = () => FLAG(true, Flag.INTERRUPT), 
        ["CLV"] = () => FLAG(true, Flag.OVERFLOW),
        
        //-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-/-//

        // CONTROL FLOW
        ["JMP"] = () => JUMP, ["JSR"] = () => CALL,
        ["RTI"] = () => RETURN(true), ["RTS"] = () => RETURN(false),
        ["BRK"] = () => BREAK,

        // PUSH & PULL
        ["PHA"] = () => PUSH(Pointer.ACC), ["PHP"] = () => PUSH(Pointer.SR),
        ["PLA"] = () => PULL(Pointer.ACC), ["PLP"] = () => PULL(Pointer.SR),

        // BRANCH CLEAR & SET
        ["BCC"] = () => BRANCH(Condition.CC), ["BCS"] = () => BRANCH(Condition.CS),
        ["BNE"] = () => BRANCH(Condition.NE), ["BEQ"] = () => BRANCH(Condition.EQ),
        ["BPL"] = () => BRANCH(Condition.PL), ["BMI"] = () => BRANCH(Condition.MI),
        ["BVC"] = () => BRANCH(Condition.VC), ["BVS"] = () => BRANCH(Condition.VS),
    };
    
    public static Signal[][] OpcodeRom(bool dump)
    {
        Signal[][] table = new Signal[256][];
        string[] lines = File.ReadAllLines("Opcodes.csv");

        for (int i = 0; i < 16; i++)
        {
            string[] cells = lines[i].Split(',').Select(x => x.Trim()).ToArray();   
            
            for (int j = 0; j < 16; j++)
            {
                string[] cell = cells[j].Split(' ');
                var index = (i << 4) | j;
                table[index] = [..AddressingTable[cell[1]](), ..MnemonicTable[cell[0]]()];
                table[index][0].Name = $"{cell[0]} {DebugAddressingModes[cell[1]]}";
            }
        }
        
        if (dump)
        {
            for (int i = 0; i < 256; i++)
            {
                Console.WriteLine($"OPCODE {i:X2}");
                foreach (Signal signal in table[i]) Console.WriteLine(signal.Cycle);
                Console.WriteLine("-----------------------------");
            }
        
            Environment.Exit(20);
        }
        
        return table;
    }
    
    private static readonly Dictionary<string, string> DebugAddressingModes = new()
    {
        {"-", "ILLEGAL"}, {"IMP", "imp"}, {"IMM", "#$??"}, {"ZP", "$??"}, {"ZPX", "$??,X"}, {"ZPY", "$??,Y"},
        {"ABS", "$????"}, {"ABSX", "$????,X"}, {"ABSY", "$????,Y"}, {"IND", "($????)"}, {"INDX", "($??,X)"}, {"INDY", "($??),Y"},
    };
}