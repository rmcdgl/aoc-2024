using AdventOfCode.Solutions.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Days;

public record ComputerState(int RegisterA, int RegisterB, int RegisterC);
public class Day17 : BaseDay<(ComputerState State, List<int> Instructions)>
{
    protected override int DayNumber => 17;



    protected override (ComputerState State, List<int> Instructions) Parse(string[] input)
    {
        var regA = int.Parse(Regex.Match(input[0], @"Register A: (\d+)").Groups[1].Value);
        var regB = int.Parse(Regex.Match(input[1], @"Register B: (\d+)").Groups[1].Value);
        var regC = int.Parse(Regex.Match(input[2], @"Register C: (\d+)").Groups[1].Value);

        var programLine = input.First(l => l.StartsWith("Program:"));
        var instructions = programLine.Substring(9)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        return (new ComputerState(regA, regB, regC), instructions);
    }

    private class Computer
    {
        private int RegisterA { get; set; }
        private int RegisterB { get; set; }
        private int RegisterC { get; set; }
        private int InstructionPointer { get; set; }
        private List<int> Program { get; set; }
        private List<int> Outputs { get; set; } = new();

        public Computer(ComputerState initialState, List<int> program)
        {
            Program = program;
            RegisterA = initialState.RegisterA;
            RegisterB = initialState.RegisterB;
            RegisterC = initialState.RegisterC;
            InstructionPointer = 0;
        }

        private int GetComboOperandValue(int operand)
        {
            return operand switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 3,
                4 => RegisterA,
                5 => RegisterB,
                6 => RegisterC,
                _ => throw new ArgumentException("Invalid combo operand")
            };
        }

        public string Execute()
        {
            while (InstructionPointer < Program.Count)
            {
                int opcode = Program[InstructionPointer];
                int operand = Program[InstructionPointer + 1];

                switch (opcode)
                {
                    case 0: // adv
                        RegisterA /= (1 << GetComboOperandValue(operand));
                        break;
                    case 1: // bxl - LITERAL
                        RegisterB ^= operand;
                        break;
                    case 2: // bst
                        RegisterB = GetComboOperandValue(operand) % 8;
                        break;
                    case 3: // jnz
                        if (RegisterA != 0)
                        {
                            InstructionPointer = operand;
                            continue; // skip instruction pointer increment
                        }
                        break;
                    case 4: // bxc
                        RegisterB ^= RegisterC;
                        break;
                    case 5: // out
                        Outputs.Add(GetComboOperandValue(operand) % 8);
                        break;
                    case 6: // bdv
                        RegisterB = RegisterA / (1 << GetComboOperandValue(operand));
                        break;
                    case 7: // cdv
                        RegisterC = RegisterA / (1 << GetComboOperandValue(operand));
                        break;
                }

                InstructionPointer += 2;
            }

            return string.Join(",", Outputs);
        }
    }

    protected override object Solve1((ComputerState State, List<int> Instructions) input)
    {
        var computer = new Computer(input.State, input.Instructions);
        return computer.Execute();
    }

    protected override object Solve2((ComputerState State, List<int> Instructions) input)
    {
        return "not implemented";
    }
}