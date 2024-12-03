using System.Text.RegularExpressions;
using AdventOfCode.Solutions.Common;

namespace AdventOfCode.Solutions.Days;

public class Day03 : BaseDay<string>
{
    private static readonly Regex Part1Regex = new(@"mul\((\d{1,3}),(\d{1,3})\)");
    private static readonly Regex Part2Regex = new(@"(?:mul\((\d{1,3}),(\d{1,3})\))|((?:do|don't)\(\))");
    
    protected override int DayNumber => 3;

    protected override string Parse(string[] input) => string.Join("", input);

    protected override object Solve1(string input) =>
        Part1Regex.Matches(input)
            .Sum(m => long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value));

    protected override object Solve2(string input)
    {
        long result = 0;
        bool enabled = true;

        foreach (Match match in Part2Regex.Matches(input))
        {
            if (match.Groups[3].Success)
            {
                enabled = match.Groups[3].Value == "do()";
            }
            else if (match.Groups[1].Success && enabled)
            {
                result += long.Parse(match.Groups[1].Value) * long.Parse(match.Groups[2].Value);
            }
        }
        
        return result;
    }
}