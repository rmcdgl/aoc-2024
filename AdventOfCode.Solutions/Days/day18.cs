using AdventOfCode.Solutions.Common;
using System.Text.RegularExpressions;

namespace AdventOfCode.Solutions.Year2024;

public class Day18 : BaseDay<List<(int Row, int Col)>>
{
    protected override int DayNumber => 18;

    private static readonly Regex CoordinatePattern = new(@"(\d+),(\d+)", RegexOptions.Compiled);
    private const int GridSize = 71; // 0 to 70 inclusive

    protected override List<(int Row, int Col)> Parse(string[] input)
    {
        return input
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var match = CoordinatePattern.Match(line);
                return (
                    Row: int.Parse(match.Groups[2].Value),
                    Col: int.Parse(match.Groups[1].Value)
                );
            })
            .ToList();
    }

    protected override object Solve1(List<(int Row, int Col)> input)
    {
        // Create grid to track corrupted spaces
        var grid = new char[GridSize, GridSize];
        
        // Initialise all cells as safe
        for (int row = 0; row < GridSize; row++)
            for (int col = 0; col < GridSize; col++)
                grid[row, col] = '.';
        
        // Mark corrupted spaces for first 1024 bytes
        for (int i = 0; i < 1024 && i < input.Count; i++)
        {
            var (row, col) = input[i];
            grid[row, col] = '#';
        }

        return FindShortestPath(grid);
    }

    private int FindShortestPath(char[,] grid)
    {
        var queue = new Queue<(int Row, int Col, int Steps)>();
        var visited = new HashSet<(int Row, int Col)>();
        
        // Start at top-left corner
        queue.Enqueue((0, 0, 0));
        visited.Add((0, 0));

        // Possible moves: down, right, up, left
        var directions = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };

        while (queue.Count > 0)
        {
            var (row, col, steps) = queue.Dequeue();

            // Check if we reached the bottom-right corner
            if (row == GridSize - 1 && col == GridSize - 1)
                return steps;

            // Try each direction
            foreach (var (dRow, dCol) in directions)
            {
                var newRow = row + dRow;
                var newCol = col + dCol;
                
                // Check if the move is valid
                if (IsInBounds(newRow, newCol, grid) && !visited.Contains((newRow, newCol)))
                {
                    queue.Enqueue((newRow, newCol, steps + 1));
                    visited.Add((newRow, newCol));
                }
            }
        }

        // If no path found
        return -1;
    }

    private bool IsInBounds(int row, int col, char[,] grid)
    {
        // Check bounds and corruption
        return row >= 0 && row < GridSize && 
               col >= 0 && col < GridSize && 
               grid[row, col] == '.';
    }

    protected override object Solve2(List<(int Row, int Col)> input)
    {
        return "not implemented";
    }
}