namespace mos6502.Signaling.Microcodes;
using Executing.Computing;

public static partial class Microcode
{
    public static Signal[][] OpcodeRom(bool dump)
    {
        Signal[][] table = new Signal[256][];
        string[] lines = File.ReadAllLines("OpcodeTable.csv");

        for (int i = 1; i <= 16; i++)
        {
            string[] cells = lines[i].Split(',').Select(x => x.Trim()).ToArray();   
            
            for (int j = 1; j <= 16; j++)
            {
                string[] cell = cells[j].Split(' ');
                var index = ((i - 1) << 4) | (j - 1);
                table[index] = [..AddressingTable[cell[1]](), ..MnemonicTable[cell[0]]()];
                if (table[index].Length < 2) continue;
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
    
    private static readonly Dictionary<string, Func<Signal[]>> AddressingTable = new()
    {
        ["-"] = () => [],
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

    private static readonly Dictionary<string, string> DebugAddressingModes = new()
    {
        {"IMP", "imp"}, {"IMM", "#$??"}, {"ZP", "$??"}, {"ZPX", "$??,X"}, {"ZPY", "$??,Y"},
        {"ABS", "$????"}, {"ABSX", "$????,X"}, {"ABSY", "$????,Y"}, {"IND", "($????)"}, {"INDX", "($??,X)"}, {"INDY", "($??),Y"},
    };
    
    private static readonly Dictionary<string, Func<Signal[]>> MnemonicTable = new()
    {
        ["-"] = () => [],
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
        ["SEC"] = () => FLAG(false, Flag.CARRY), ["SED"] = () => FLAG(false, Flag.DECIMAL), ["SEI"] = () => FLAG(false, Flag.INTERRUPT),
        ["CLC"] = () => FLAG(true, Flag.CARRY), ["CLD"] = () => FLAG(true, Flag.DECIMAL), ["CLI"] = () => FLAG(true, Flag.INTERRUPT), 
        ["CLV"] = () => FLAG(true, Flag.OVERFLOW),
        
        // ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||

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
}