using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2024;

public class Day06 : BaseDay<char[][]>
{
    protected override int DayNumber => 6;

    private enum Direction
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3
    }

    private static readonly (int Row, int Col)[] Moves = new[]
    {
        (-1, 0),  // Up
        (0, 1),   // Right
        (1, 0),   // Down
        (0, -1)   // Left
    };

    protected override char[][] Parse(string[] input)
    {
        return input.Select(line => line.ToCharArray()).ToArray();
    }

    protected override object Solve1(char[][] grid)
    {
        var (startPos, startDir) = FindStartPosition(grid);
        var visited = new HashSet<(int Row, int Col)>();
        var current = (startPos.Row, startPos.Col, startDir);

        while (IsInBounds(current.Row, current.Col, grid))
        {
            visited.Add((current.Row, current.Col));
            var nextPos = GetNextPosition(current, grid);
            
            if (!nextPos.HasValue)
                break;
                
            current = nextPos.Value;
        }

        return visited.Count;
    }

    protected override object Solve2(char[][] grid)
    {
        var (startPos, startDir) = FindStartPosition(grid);
        int loopCount = 0;

        // Try each empty position
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[0].Length; col++)
            {
                // Skip positions that are already occupied or the start position
                if (grid[row][col] != '.' || (row == startPos.Row && col == startPos.Col))
                    continue;

                // Place obstacle and check if it creates a loop
                grid[row][col] = '#';
                if (CreatesLoop(grid, (startPos.Row, startPos.Col, startDir)))
                {
                    loopCount++;
                }
                grid[row][col] = '.';  // Reset the position
            }
        }

        return loopCount;
    }

    private bool CreatesLoop(char[][] grid, (int Row, int Col, Direction Dir) start)
    {
        var visited = new HashSet<(int Row, int Col, Direction Dir)>();
        var current = start;

        while (true)
        {
            // If we've seen this exact state before, we found a loop
            if (!visited.Add(current))
                return true;

            // Get next position
            var nextPos = GetNextPosition(current, grid);

            // If we're out of bounds or have visited too many positions, no loop
            if (!nextPos.HasValue || visited.Count > grid.Length * grid[0].Length * 4)
                return false;

            current = nextPos.Value;
        }
    }

    private ((int Row, int Col), Direction) FindStartPosition(char[][] grid)
    {
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {
                switch (grid[row][col])
                {
                    case '^': return ((row, col), Direction.Up);
                    case '>': return ((row, col), Direction.Right);
                    case 'v': return ((row, col), Direction.Down);
                    case '<': return ((row, col), Direction.Left);
                }
            }
        }
        throw new InvalidOperationException("No start position found");
    }

    private (int Row, int Col, Direction Dir)? GetNextPosition((int Row, int Col, Direction Dir) current, char[][] grid)
    {
        var (row, col, dir) = current;
        
        // Check what's in front of us
        var move = Moves[(int)dir];
        var nextRow = row + move.Row;
        var nextCol = col + move.Col;

        // If we would step out of bounds, we're done
        if (!IsInBounds(nextRow, nextCol, grid))
        {
            return null;
        }

        // If the path ahead is clear, move forward with same direction
        if (grid[nextRow][nextCol] != '#')
        {
            return (nextRow, nextCol, dir);
        }
        
        // If there's an obstacle, just turn right and stay put
        var newDir = (Direction)(((int)dir + 1) % 4);
        return (row, col, newDir);
    }

    private bool IsInBounds(int row, int col, char[][] grid) =>
        row >= 0 && row < grid.Length && col >= 0 && col < grid[0].Length;
}