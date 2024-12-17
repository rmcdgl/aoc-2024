using AdventOfCode.Solutions.Common;
using System.Text.RegularExpressions;
    
using System;
using System.Diagnostics;
using System.Numerics;

namespace AdventOfCode.Solutions.Days;

public record ComputerState(long RegisterA, long RegisterB, long RegisterC);

public class Day17 : BaseDay<(ComputerState State, List<int> Instructions)>
{
    protected override int DayNumber => 17;

    protected override (ComputerState State, List<int> Instructions) Parse(string[] input)
    {
        var regA = long.Parse(Regex.Match(input[0], @"Register A: (\d+)").Groups[1].Value);
        var regB = long.Parse(Regex.Match(input[1], @"Register B: (\d+)").Groups[1].Value);
        var regC = long.Parse(Regex.Match(input[2], @"Register C: (\d+)").Groups[1].Value);

        var programLine = input.First(l => l.StartsWith("Program:"));
        var instructions = programLine.Substring(9)
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

        return (new ComputerState(regA, regB, regC), instructions);
    }

    private class Computer
    {
        private long RegisterA { get; set; }
        private long RegisterB { get; set; }
        private long RegisterC { get; set; }
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

        private long GetComboOperandValue(int operand)
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
                        RegisterA /= (1L << (int)GetComboOperandValue(operand));
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
                        Outputs.Add((int)(GetComboOperandValue(operand) % 8));
                        break;
                    case 6: // bdv
                        RegisterB = RegisterA / (1L << (int)GetComboOperandValue(operand));
                        break;
                    case 7: // cdv
                        RegisterC = RegisterA / (1L << (int)GetComboOperandValue(operand));
                        break;
                }

                InstructionPointer += 2;
            }

            return string.Join(",", Outputs);
        }

        public List<int> GetOutputs()
        {
            Execute();
            return Outputs;
        }
    }

    protected override object Solve1((ComputerState State, List<int> Instructions) input)
    {
        var computer = new Computer(input.State, input.Instructions);
        return computer.Execute();
    }
    
    public static int GetLongestPrefixLength(string str1, string str2)
    {
        int minLength = Math.Min(str1.Length, str2.Length);
        int i = 0;
    
        while (i < minLength && str1[i] == str2[i])
        {
            i++;
        }
    
        return i;
    }

    
    //  10  011 010 101 011 110 111 110 100
            // 3   1    5   7   2    1    4    2
            
            
    // length: 10000000000000000000000000000000000000000000000
    // length: 1000000000000000000010011010101011110111110100 == 35184412638708
    
    // protected override object Solve2((ComputerState State, List<int> Instructions) input)
    // {
    //     int instructionMatches = 0;
    //     long baseValue = 40549876; // Our known partially working value
    //     int knownBits = 26; // Length of 10011010101011110111110100
    //
    //     // Create a mask for the bits we want to keep
    //     long preserveMask = (1L << knownBits) - 1;
    //
    //     // Loop through possibilities for higher bits while preserving lower bits
    //     for (long i = baseValue; i < long.MaxValue; i += (1L << knownBits))
    //     {
    //         var computer = new Computer(new ComputerState(i, 0, 0), input.Instructions);
    //         var output = computer.Execute();
    //         var longestPrefixLength = GetLongestPrefixLength(output, string.Join(",", input.Instructions));
    //         if (longestPrefixLength > instructionMatches)
    //         {
    //             instructionMatches = longestPrefixLength;
    //             Console.WriteLine($"output length:{longestPrefixLength} output: {output} register A:{i} binary: {Convert.ToString(i, 2)}");
    //         }
    //     }
    //     return "done";
    // }
    // output length:16 output: 2,4,1,2,7,5,1,3,1 register A:40549876 binary: 10011010101011110111110100
    // output length:18 output: 2,4,1,2,7,5,1,3,4,7 register A:912965108 binary: 110110011010101011110111110100
    // output length:20 output: 2,4,1,2,7,5,1,3,4,4,7 register A:7355416052 binary: 110110110011010101011110111110100
    // output length:23 output: 2,4,1,2,7,5,1,3,4,4,5,5 register A:41715154420 binary: 100110110110011010101011110111110100
    // output length:24 output: 2,4,1,2,7,5,1,3,4,4,5,5,1 register A:179154107892 binary: 10100110110110011010101011110111110100
    // output length:28 output: 2,4,1,2,7,5,1,3,4,4,5,5,0,3,4 register A:22306825616884 binary: 101000100100110110110011010101011110111110100
    // output length:30 output: 2,4,1,2,7,5,1,3,4,4,5,5,0,3,3,5,7,0 register A:3962956499566068 binary: 1110000101000100100110110110011010101011110111110100
    // output length:31 output: 2,4,1,2,7,5,1,3,4,4,5,5,0,3,3,0,7,0 register A:4174062732099060 binary: 1110110101000100100110110110011010101011110111110100

    
    // 1000000001110001010110010000101011110111110100
    // 1000000000000000000000000000000000000000000000
    // protected override object Solve2((ComputerState State, List<int> Instructions) input)
    // {
    //     int instructionMatches = 0;
    //     string target = string.Join(",", input.Instructions);
    //     string baseBits = "0101011110111110100";
    //     Console.WriteLine($"target: {target} - target.Length: {target.Length}");
    //     for (long i = 35193829000000; i < long.MaxValue; i++)
    //     {
    //         
    //         var computer = new Computer(new ComputerState(i,input.State.RegisterB, input.State.RegisterC), input.Instructions);
    //         string output = computer.Execute();
    //
    //         if (i % 1000000 == 0)
    //         {
    //             Console.WriteLine($"{i}: {output} - output length: {output.Length}");
    //         }
    //
    //         if (output.Length > target.Length)
    //         {
    //             throw new Exception();
    //         }
    //
    //         if (target == output)
    //         {
    //             Console.WriteLine($"Found a value: {i}");
    //             Debugger.Break();
    //             return i;
    //         }
    //     }
    //
    //     
    //     return "No exact match found";
    // }
    
    // prefix
    protected override object Solve2((ComputerState State, List<int> Instructions) input)
    {
        string target = string.Join(",", input.Instructions);
        var test = 0L; //tested possible inputs agian
    
        Console.WriteLine($"target: {target} - target.Length: {target.Length}");
        var output = new Computer(new ComputerState(test, 0, 0), input.Instructions).Execute();
        Console.WriteLine($"output: {output} - {output == target}");
        return "done";

        // for (long i = 0; i < long.MaxValue; i++)
        // {
        //     var computer = new Computer(new ComputerState(i, input.State.RegisterB, input.State.RegisterC), input.Instructions);
        //     string output = computer.Execute();
        //
        //     // if (i % 1000000 == 0)
        //     // {
        //     //     Console.WriteLine($"{i}: {output} - output length: {output.Length}");
        //     // }
        //
        //     if (output.Length > target.Length)
        //     {
        //         throw new Exception();
        //     }
        //
        //     // Check if output is a prefix of target
        //     if (target.StartsWith(output))
        //     {
        //         Console.WriteLine($"Prefix match found at {i}");
        //         Console.WriteLine($"Decimal: {i}");
        //         Console.WriteLine($"Binary:  {Convert.ToString(i, 2)}");
        //         Console.WriteLine($"Output:  {output}");
        //         Console.WriteLine("------------------");
        //     }
        //
        //     if (target == output)
        //     {
        //         Console.WriteLine($"Found exact match: {i}");
        //         Console.WriteLine($"Binary: {Convert.ToString(i, 2)}");
        //         Debugger.Break();
        //         return i;
        //     }
        // }
        //
        // return "No exact match found";
    }

    //suffix
    // protected override object Solve2((ComputerState State, List<int> Instructions) input)
    // {
    //     string target = string.Join(",", input.Instructions);
    //
    //     Console.WriteLine($"target: {target} - target.Length: {target.Length}");
    //
    //     for (long i = 0; i < long.MaxValue; i++)
    //     {
    //         var computer = new Computer(new ComputerState(i, input.State.RegisterB, input.State.RegisterC), input.Instructions);
    //         string output = computer.Execute();
    //
    //         // if (i % 1000000 == 0)
    //         // {
    //         //     Console.WriteLine($"{i}: {output} - output length: {output.Length}");
    //         // }
    //
    //         if (output.Length > target.Length)
    //         {
    //             throw new Exception();
    //         }
    //
    //         // Check if output is a prefix of target
    //         if (target.StartsWith(output))
    //         {
    //             Console.WriteLine($"Prefix match found at {i}");
    //             Console.WriteLine($"Decimal: {i}");
    //             Console.WriteLine($"Binary:  {Convert.ToString(i, 2)}");
    //             Console.WriteLine($"Output:  {output}");
    //             Console.WriteLine("------------------");
    //         }
    //
    //         // Check if output is a suffix of target
    //         if (target.EndsWith(output))
    //         {
    //             Console.WriteLine($"Suffix match found at {i}");
    //             Console.WriteLine($"Decimal: {i}");
    //             Console.WriteLine($"Binary:  {Convert.ToString(i, 2)}");
    //             Console.WriteLine($"Output:  {output}");
    //             Console.WriteLine("------------------");
    //         }
    //
    //         if (target == output)
    //         {
    //             Console.WriteLine($"Found exact match: {i}");
    //             Console.WriteLine($"Binary: {Convert.ToString(i, 2)}");
    //             Debugger.Break();
    //             return i;
    //         }
    //     }
    //
    //     return "No exact match found";
    // }
    
    // ------------------
    // Suffix match found at 8
    // Decimal: 8
    // Binary:  1000
    // Output:  3,0
    // ------------------
    // Suffix match found at 67
    // Decimal: 67
    // Binary:  1000011
    // Output:  3,3,0
    // ------------------
    // Suffix match found at 70
    // Decimal: 70
    // Binary:  1000110
    // Output:  3,3,0
    // ------------------
    // Suffix match found at 541
    // Decimal: 541
    // Binary:  1000011101
    // Output:  0,3,3,0
    // ------------------
    // Suffix match found at 565
    // Decimal: 565
    // Binary:  1000110101
    // Output:  0,3,3,0
    // ------------------
    // Suffix match found at 4329
    // Decimal: 4329
    // Binary:  1000011101001
    // Output:  5,0,3,3,0
    // ------------------
    // Suffix match found at 4333
    // Decimal: 4333
    // Binary:  1000011101101
    // Output:  5,0,3,3,0
    // ------------------
    // Suffix match found at 4521
    // Decimal: 4521
    // Binary:  1000110101001
    // Output:  5,0,3,3,0
    // ------------------
    // Suffix match found at 4526
    // Decimal: 4526
    // Binary:  1000110101110
    // Output:  5,0,3,3,0
    // ------------------
    // Suffix match found at 34665
    // Decimal: 34665
    // Binary:  1000011101101001
    // Output:  5,5,0,3,3,0
    // ------------------
    // Suffix match found at 34671
    // Decimal: 34671
    // Binary:  1000011101101111
    // Output:  5,5,0,3,3,0
    // ------------------
    // Suffix match found at 36208
    // Decimal: 36208
    // Binary:  1000110101110000
    // Output:  5,5,0,3,3,0
    // ------------------
    // Suffix match found at 36215
    // Decimal: 36215
    // Binary:  1000110101110111
    // Output:  5,5,0,3,3,0
    // ------------------
    // Suffix match found at 277327
    // Decimal: 277327
    // Binary:  1000011101101001111 011010101011110111110100
    // Output:  4,5,5,0,3,3,0
    // ------------------
    // Suffix match found at 289726
    // Decimal: 289726
    // Binary:  1000110101110111110 0011010101011110111110100
    // Output:  4,5,5,0,3,3,0
    // ------------------
    // Suffix match found at 2218620
    // Decimal: 2218620
    // Binary:  1000011101101001111100 011010101011110111110100
    // Output:  4,4,5,5,0,3,3,0
    // ------------------
    // Prefix match found at 6995444
    // Decimal: 6995444
    // Binary:  11010101011110111110100
    // Output:  2,4,1,2,7,5,1,3
    // ------------------
    // Suffix match found at 17748963
    // Decimal: 17748963
    // Binary:  1000011101101001111100011
    // Output:  3,4,4,5,5,0,3,3,0
    // ------------------
    // Suffix match found at 17748965
    // Decimal: 17748965
    // Binary:  1000011101101001111100101
    // Output:  3,4,4,5,5,0,3,3,0
    // ------------------
    // 
    
    // Prefix match found at 6995444
    // Decimal: 6995444
    // Binary:  11010101011110111110100
    // Output:  2,4,1,2,7,5,1,3
    // ------------------
    // 
}