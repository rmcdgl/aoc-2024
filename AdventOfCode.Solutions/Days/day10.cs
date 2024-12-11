using AdventOfCode.Solutions.Common;

namespace AdventOfCode.Solutions.Year2024;

public class Day10 : BaseDay<int[][]>
{
    protected override int DayNumber => 10;
    
    protected override int[][] Parse(string[] input)
    {
        return input.Select(line => 
            line.Select(c => c - '0').ToArray()
        ).ToArray();
    }

    private record struct Position(int Row, int Col);

    private readonly (int Row, int Col)[] Directions = new[]
    {
        (-1, 0), // Up
        (1, 0),  // Down
        (0, -1), // Left
        (0, 1)   // Right
    };

    private bool IsInBounds(int row, int col, int height, int width)
    {
        return row >= 0 && row < height && col >= 0 && col < width;
    }

    private HashSet<Position> FindReachableNines(int[][] grid, Position start)
    {
        int height = grid.Length;
        int width = grid[0].Length;
        var reachableNines = new HashSet<Position>();
        var visited = new HashSet<Position>();
        var queue = new Queue<(Position Pos, int Height)>();
        queue.Enqueue((start, grid[start.Row][start.Col]));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (currentPos, currentHeight) = queue.Dequeue();

            if (grid[currentPos.Row][currentPos.Col] == 9)
            {
                reachableNines.Add(currentPos);
                continue;
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var newPos = new Position(currentPos.Row + dRow, currentPos.Col + dCol);

                if (!IsInBounds( newPos.Row, newPos.Col, height, width ) || visited.Contains(newPos))
                    continue;

                var newHeight = grid[newPos.Row][newPos.Col];
                if (newHeight == currentHeight + 1)
                {
                    queue.Enqueue((newPos, newHeight));
                    visited.Add(newPos);
                }
            }
        }

        return reachableNines;
    }
    
    protected override object Solve1(int[][] input)
    {
        var totalScore = 0;
        
        // Find all trailheads (positions with height 0)
        for (int row = 0; row < input.Length; row++)
        {
            for (int col = 0; col < input[0].Length; col++)
            {
                if (input[row][col] == 0)
                {
                    var reachableNines = FindReachableNines(input, new Position(row, col));
                    totalScore += reachableNines.Count;
                }
            }
        }

        return totalScore;
    }
    
    protected override object Solve2(int[][] input)
    {
        return "not implemented";
    }
}