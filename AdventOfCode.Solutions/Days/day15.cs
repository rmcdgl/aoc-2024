using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Solutions.Year2024;

public class Day15 : BaseDay<(char[][] Grid, string Moves)>
{
    protected override int DayNumber => 15;
    
    protected override (char[][] Grid, string Moves) Parse(string[] input)
    {
        // Find the empty line that separates grid from moves
        int emptyLineIndex = Array.FindIndex(input, string.IsNullOrWhiteSpace);
        
        // Parse the grid
        var grid = input.Take(emptyLineIndex)
            .Select(line => line.ToArray())
            .ToArray();
            
        // Parse moves by concatenating all remaining lines and filtering valid moves
        var movesBuilder = new StringBuilder();
        foreach (var line in input.Skip(emptyLineIndex + 1))
        {
            foreach (char c in line)
            {
                if (c is '^' or 'v' or '<' or '>')
                {
                    movesBuilder.Append(c);
                }
            }
        }
        var moves = movesBuilder.ToString();
        
        return (grid, moves);
    }
    
    private static (int Row, int Col) FindRobot(char[][] grid)
    {
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] == '@')
                    return (row, col);
            }
        }
        throw new Exception("Robot not found");
    }
    
    private static bool IsInBounds(int row, int col, char[][] grid)
    {
        return row >= 0 && row < grid.Length && col >= 0 && col < grid[0].Length;
    }
    
    private static void MoveRobot(char[][] grid, char direction)
    {
        var (dRow, dCol) = direction switch
        {
            '^' => (-1, 0),
            'v' => (1, 0),
            '<' => (0, -1),
            '>' => (0, 1),
            _ => throw new Exception($"Invalid direction: {direction}")
        };
        
        var (robotRow, robotCol) = FindRobot(grid);
        int targetRow = robotRow + dRow;
        int targetCol = robotCol + dCol;
        
        // Check bounds for initial move
        if (!IsInBounds(targetRow, targetCol, grid))
            return;
            
        // If the target is a wall, do nothing
        if (grid[targetRow][targetCol] == '#') 
            return;

        // If the target is empty, just move
        if (grid[targetRow][targetCol] == '.')
        {
            grid[robotRow][robotCol] = '.';
            grid[targetRow][targetCol] = '@';
            return;
        }

        // If the target is a box, find the chain of boxes
        var boxPositions = new List<(int r, int c)>();
        int currentRow = targetRow;
        int currentCol = targetCol;

        // Move along the direction until a non-box is found or we hit bounds
        while (IsInBounds(currentRow, currentCol, grid) && grid[currentRow][currentCol] == 'O')
        {
            boxPositions.Add((currentRow, currentCol));
            currentRow += dRow;
            currentCol += dCol;
        }

        // Check if we can push the chain of boxes
        if (!IsInBounds(currentRow, currentCol, grid) || grid[currentRow][currentCol] != '.')
            return;

        // Push all boxes forward
        // Start from the last box in the chain and move forward to avoid overwriting
        for (int i = boxPositions.Count - 1; i >= 0; i--)
        {
            var (br, bc) = boxPositions[i];
            grid[br + dRow][bc + dCol] = 'O';
            grid[br][bc] = '.';
        }

        // Move the robot
        grid[robotRow][robotCol] = '.';
        grid[targetRow][targetCol] = '@';
    }
    
    private static int CalculateGPS(char[][] grid)
    {
        int sum = 0;
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[0].Length; col++)
            {
                if (grid[row][col] == 'O')
                {
                    sum += (100 * row) + col;
                }
            }
        }
        return sum;
    }
    
    protected override object Solve1((char[][] Grid, string Moves) input)
    {
        var grid = input.Grid.Select(row => row.ToArray()).ToArray(); // Create a copy
        
        foreach (char move in input.Moves)
        {
            MoveRobot(grid, move);
        }
        
        return CalculateGPS(grid);
    }
    
    protected override object Solve2((char[][] Grid, string Moves) input)
    {
        return "not implemented";
    }
}