using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2024;

public class Day05 : BaseDay<(List<(int Before, int After)> Rules, List<List<int>> Updates)>
{
    protected override int DayNumber => 5;

    private static readonly Regex NumberPattern = new(@"(\d+)(?:\|(\d+)|(?:,|$))", RegexOptions.Compiled);

    protected override (List<(int Before, int After)> Rules, List<List<int>> Updates) Parse(string[] input)
    {
        var rules = new List<(int Before, int After)>();
        var updates = new List<List<int>>();
        
        bool parsingRules = true;
        
        foreach (var line in input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                parsingRules = false;
                continue;
            }

            if (parsingRules)
            {
                var match = NumberPattern.Match(line);
                if (match.Success && match.Groups[2].Success)
                {
                    rules.Add((
                        int.Parse(match.Groups[1].Value),
                        int.Parse(match.Groups[2].Value)
                    ));
                }
            }
            else
            {
                var numbers = NumberPattern.Matches(line)
                    .Select(m => int.Parse(m.Groups[1].Value))
                    .ToList();
                if (numbers.Any())
                {
                    updates.Add(numbers);
                }
            }
        }

        return (rules, updates);
    }

    private bool IsValidOrder(List<int> update, List<(int Before, int After)> rules)
    {
        foreach (var rule in rules)
        {
            if (!update.Contains(rule.Before) || !update.Contains(rule.After))
                continue;

            int beforeIndex = update.IndexOf(rule.Before);
            int afterIndex = update.IndexOf(rule.After);

            if (beforeIndex > afterIndex)
                return false;
        }

        return true;
    }

    private List<int> SortUpdate(List<int> update, List<(int Before, int After)> rules)
    {
        var result = update.ToList();
        bool swapped;
        
        do
        {
            swapped = false;
            for (int i = 0; i < result.Count - 1; i++)
            {
                bool shouldSwap = false;
                
                // Check if any rule says these two numbers should be in different order
                foreach (var rule in rules)
                {
                    // If current pair matches a rule's before/after
                    if (result[i] == rule.After && result[i + 1] == rule.Before)
                    {
                        shouldSwap = true;
                        break;
                    }
                }
                
                if (shouldSwap)
                {
                    // Swap the elements
                    (result[i], result[i + 1]) = (result[i + 1], result[i]);
                    swapped = true;
                }
            }
        } while (swapped);

        return result;
    }

    private int GetMiddlePage(List<int> update)
    {
        return update[update.Count / 2];
    }

    protected override object Solve1((List<(int Before, int After)> Rules, List<List<int>> Updates) input)
    {
        var validUpdates = input.Updates
            .Where(update => IsValidOrder(update, input.Rules))
            .ToList();

        return validUpdates
            .Select(GetMiddlePage)
            .Sum();
    }

    protected override object Solve2((List<(int Before, int After)> Rules, List<List<int>> Updates) input)
    {
        var invalidUpdates = input.Updates
            .Where(update => !IsValidOrder(update, input.Rules))
            .ToList();

        return invalidUpdates
            .Select(update => SortUpdate(update, input.Rules))
            .Select(GetMiddlePage)
            .Sum();
    }
}