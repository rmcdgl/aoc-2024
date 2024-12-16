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

    private int FindShortestPath(char[][] grid, (int row, int col) start, (int row, int col) end, Direction initialDir)
    {
        var distances = new Dictionary<State, int>();
        var queue = new PriorityQueue<State, int>();
        
        var initialState = new State(start.row, start.col, initialDir);
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
    
    private record struct PathInfo
    {
        public Dictionary<State, int> Distances { get; init; }
        public Dictionary<State, HashSet<State>> Predecessors { get; init; }
        public int MinEndCost { get; init; }
    }

    private PathInfo FindAllOptimalPaths(char[][] grid, (int row, int col) start, (int row, int col) end, Direction initialDir)
    {
        var distances = new Dictionary<State, int>();
        var predecessors = new Dictionary<State, HashSet<State>>();
        var queue = new PriorityQueue<State, int>();
        
        var initialState = new State(start.row, start.col, initialDir);
        distances[initialState] = 0;
        predecessors[initialState] = new HashSet<State>();
        queue.Enqueue(initialState, 0);

        int? minEndCost = null;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var currentCost = distances[current];

            if (current.Row == end.row && current.Col == end.col)
            {
                if (minEndCost == null || currentCost == minEndCost)
                {
                    minEndCost = currentCost;
                }
                continue;
            }

            if (minEndCost.HasValue && currentCost > minEndCost.Value)
                continue;

            foreach (var nextDir in GetNextDirections(current.Dir))
            {
                var turnCost = nextDir == current.Dir ? 0 : 1000;
                var (dRow, dCol) = Moves[nextDir];
                var newRow = current.Row + dRow;
                var newCol = current.Col + dCol;

                if (!IsInBounds(grid, newRow, newCol))
                    continue;

                var nextState = new State(newRow, newCol, nextDir);
                var newCost = currentCost + turnCost + 1;

                if (!distances.ContainsKey(nextState))
                {
                    distances[nextState] = newCost;
                    predecessors[nextState] = new HashSet<State> { current };
                    queue.Enqueue(nextState, newCost);
                }
                else if (newCost == distances[nextState])
                {
                    predecessors[nextState].Add(current);
                }
                else if (newCost < distances[nextState])
                {
                    distances[nextState] = newCost;
                    predecessors[nextState] = new HashSet<State> { current };
                    queue.Enqueue(nextState, newCost);
                }
            }
        }

        if (!minEndCost.HasValue)
            throw new Exception("No path found");

        return new PathInfo
        {
            Distances = distances,
            Predecessors = predecessors,
            MinEndCost = minEndCost.Value
        };
    }

    private HashSet<(int row, int col)> GetOptimalPathTiles(PathInfo pathInfo, (int row, int col) start, (int row, int col) end)
    {
        var optimalTiles = new HashSet<(int row, int col)>();

        // Find all end states that have the optimal cost
        var optimalEndStates = pathInfo.Distances
            .Where(kvp => kvp.Key.Row == end.row && kvp.Key.Col == end.col && kvp.Value == pathInfo.MinEndCost)
            .Select(kvp => kvp.Key);

        foreach (var endState in optimalEndStates)
        {
            var stack = new Stack<State>();
            stack.Push(endState);
            optimalTiles.Add((endState.Row, endState.Col));

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (pathInfo.Predecessors.TryGetValue(current, out var predecessors))
                {
                    foreach (var pred in predecessors)
                    {
                        optimalTiles.Add((pred.Row, pred.Col));
                        stack.Push(pred);
                    }
                }
            }
        }

        return optimalTiles;
    }
    
    protected override object Solve1(char[][] grid)
    {
        var start = FindPosition(grid, 'S');
        var end = FindPosition(grid, 'E');
        return FindShortestPath(grid, start, end, Direction.East);
    }
    
    protected override object Solve2(char[][] grid)
    {
        var start = FindPosition(grid, 'S');
        var end = FindPosition(grid, 'E');
        var pathInfo = FindAllOptimalPaths(grid, start, end, Direction.East);
        return GetOptimalPathTiles(pathInfo, start, end).Count;
    }
}