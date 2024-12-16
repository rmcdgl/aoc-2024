using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2024;

public class Day16 : BaseDay<char[][]>
{
    protected override int DayNumber => 16;
    
    protected override char[][] Parse(string[] input)
    {
        return input.Select(line => 
            line.ToArray()
        ).ToArray();
    }

    private record struct State(int Row, int Col, Direction Dir);
    
    private enum Direction
    {
        North,
        East,
        South,
        West
    }

    private static readonly Dictionary<Direction, (int dRow, int dCol)> Moves = new()
    {
        { Direction.North, (-1, 0) },
        { Direction.East, (0, 1) },
        { Direction.South, (1, 0) },
        { Direction.West, (0, -1) }
    };

    private static (int row, int col) FindPosition(char[][] grid, char position)
    {
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] == position)
                    return (i, j);
            }
        }
        throw new Exception("Position not found");
    }
    
    private static bool IsInBounds(char[][] grid, int row, int col)
    {
        return row >= 0 && row < grid.Length && 
               col >= 0 && col < grid[0].Length && 
               grid[row][col] != '#';
    }

    private static Direction[] GetNextDirections(Direction current)
    {
        return new[] 
        {
            current,  // Continue straight
            (Direction)(((int)current + 1) % 4),  // Turn right
            (Direction)(((int)current + 3) % 4)   // Turn left
        };
    }

    private int FindShortestPath(char[][] grid)
    {
        var start = FindPosition(grid, 'S');
        var end = FindPosition(grid, 'E');

        var distances = new Dictionary<State, int>();
        var queue = new PriorityQueue<State, int>();
        
        // Start facing East
        var initialState = new State(start.row, start.col, Direction.East);
        distances[initialState] = 0;
        queue.Enqueue(initialState, 0);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = distances[current];

            if (current.Row == end.row && current.Col == end.col)
            {
                return currentCost;
            }

            foreach (var nextDir in GetNextDirections(current.Dir))
            {
                var turnCost = nextDir == current.Dir ? 0 : 1000;
                var (dRow, dCol) = Moves[nextDir];
                var newRow = current.Row + dRow;
                var newCol = current.Col + dCol;

                if (!IsInBounds(grid, newRow, newCol))
                    continue;

                var nextState = new State(newRow, newCol, nextDir);
                var newCost = currentCost + turnCost + 1;  // +1 for moving to the new position

                if (!distances.ContainsKey(nextState) || newCost < distances[nextState])
                {
                    distances[nextState] = newCost;
                    queue.Enqueue(nextState, newCost);
                }
            }
        }

        throw new Exception("No path found");
    }
    
    protected override object Solve1(char[][] grid)
    {
        return FindShortestPath(grid);
    }
    
    protected override object Solve2(char[][] grid)
    {
        return "not implemented";
    }
}