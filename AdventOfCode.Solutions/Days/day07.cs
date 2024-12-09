using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2024;

public class Day07 : BaseDay<List<(long testValue, List<int> numbers)>>
{
    protected override int DayNumber => 7;
    
    protected override List<(long testValue, List<int> numbers)> Parse(string[] input)
    {
        return input.Select(line =>
        {
            var parts = line.Split(':');
            var testValue = long.Parse(parts[0].Trim());
            var numbers = parts[1].Trim().Split(' ').Select(int.Parse).ToList();
            return (testValue, numbers);
        }).ToList();
    }

    private long EvaluateExpression(List<int> numbers, List<char> operators)
    {
        // Start with the first number
        long result = numbers[0];
        
        // Process each operator left to right
        for (int i = 0; i < operators.Count; i++)
        {
            var nextNum = numbers[i + 1];
            switch (operators[i])
            {
                case '+':
                    result += nextNum;
                    break;
                case '*':
                    result *= nextNum;
                    break;
                case '|': // Concatenation
                    result = long.Parse($"{result}{nextNum}");
                    break;
            }
        }
        
        return result;
    }

    private bool CanMakeValue(long target, List<int> numbers, bool includeConcatenation)
    {
        int operatorsNeeded = numbers.Count - 1;
        var operators = includeConcatenation ? 
            new[] { '+', '*', '|' } :  // '|' represents concatenation
            new[] { '+', '*' };
        
        // Generate all possible combinations using base-3 or base-2 counting
        int maxCombinations = (int)Math.Pow(operators.Length, operatorsNeeded);
        
        for (int i = 0; i < maxCombinations; i++)
        {
            var combination = new List<char>();
            int temp = i;
            
            for (int j = 0; j < operatorsNeeded; j++)
            {
                combination.Add(operators[temp % operators.Length]);
                temp /= operators.Length;
            }
            
            try
            {
                if (EvaluateExpression(numbers, combination) == target)
                    return true;
            }
            catch
            {
                // Skip invalid combinations (e.g., overflow)
                continue;
            }
        }

        return false;
    }

    protected override object Solve1(List<(long testValue, List<int> numbers)> equations)
    {
        long sum = 0;
        foreach (var (testValue, numbers) in equations)
        {
            if (CanMakeValue(testValue, numbers, includeConcatenation: false))
            {
                sum += testValue;
            }
        }
        return sum;
    }

    protected override object Solve2(List<(long testValue, List<int> numbers)> equations)
    {
        long sum = 0;
        foreach (var (testValue, numbers) in equations)
        {
            if (CanMakeValue(testValue, numbers, includeConcatenation: true))
            {
                sum += testValue;
            }
        }
        return sum;
    }
}