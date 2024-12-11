using System.Diagnostics;

using AdventOfCode.Solutions.Common;

namespace AdventOfCode.Solutions.Year2024
{
    public class Day11 : BaseDay<List<long>>
    {
        protected override int DayNumber => 11;
        
        protected override List<long> Parse(string[] input)
        {
            return input[0].Split(' ').Select(long.Parse).ToList();
        }

        private List<long> ProcessStone(long stone)
        {
            // Rule 1: If stone is 0, replace with 1
            if (stone == 0)
                return new List<long> { 1 };

            // Rule 2: If stone has even number of digits, split it
            string digits = stone.ToString();
            if (digits.Length > 1 && digits.Length % 2 == 0)
            {
                int mid = digits.Length / 2;
                string leftHalf = digits.Substring(0, mid);
                string rightHalf = digits.Substring(mid);
                
                long leftNum = long.Parse(leftHalf);
                long rightNum = long.Parse(rightHalf);
                
                return new List<long> { leftNum, rightNum };
            }

            // Rule 3: Multiply by 2024
            return new List<long> { stone * 2024 };
        }

        private List<long> ProcessBlink(List<long> stones)
        {
            List<long> result = new List<long>();
            
            foreach (var stone in stones)
            {
                result.AddRange(ProcessStone(stone));
            }
            
            return result;
        }

        protected override object Solve1(List<long> input)
        {
            var currentStones = input;
            
            for (int i = 0; i < 25; i++)
            {
                currentStones = ProcessBlink(currentStones);
            }
            
            return currentStones.Count;
        }
        
        protected override object Solve2(List<long> input)
        {
            return "not implemented";
        }
    }
}