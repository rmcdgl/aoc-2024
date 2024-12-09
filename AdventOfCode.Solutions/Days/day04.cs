using AdventOfCode.Solutions.Common;

namespace AdventOfCode.Solutions.Year2024;

public class Day04 : BaseDay<char[][]>
{
    protected override int DayNumber => 4;

    protected override char[][] Parse(string[] input)
    {
        return input.Select(line => line.ToCharArray()).ToArray();
    }

    protected override object Solve1(char[][] input)
    {
        int count = 0;
        int rows = input.Length;
        int cols = input[0].Length;
        
        // All possible directions: horizontal, vertical, and diagonal
        (int dr, int dc)[] directions = new[]
        {
            (-1, -1), (-1, 0), (-1, 1),  // Up-left, Up, Up-right
            (0, -1),           (0, 1),    // Left, Right
            (1, -1),  (1, 0),  (1, 1)     // Down-left, Down, Down-right
        };

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                foreach (var (dr, dc) in directions)
                {
                    // Check if XMAS can start from this position in this direction
                    if (CanFormXMAS(input, row, col, dr, dc, rows, cols))
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    private bool CanFormXMAS(char[][] grid, int startRow, int startCol, int dr, int dc, int rows, int cols)
    {
        string target = "XMAS";
        
        // Check if the word would go out of bounds
        if (!IsInBounds(startRow + 3 * dr, startCol + 3 * dc, rows, cols))
        {
            return false;
        }

        // Check each character
        for (int i = 0; i < target.Length; i++)
        {
            int currentRow = startRow + (i * dr);
            int currentCol = startCol + (i * dc);
            
            if (grid[currentRow][currentCol] != target[i])
            {
                return false;
            }
        }

        return true;
    }

    protected override object Solve2(char[][] input)
    {
        int count = 0;
        int rows = input.Length;
        int cols = input[0].Length;

        // For each potential center point of the X
        for (int row = 1; row < rows - 1; row++)
        {
            for (int col = 1; col < cols - 1; col++)
            {
                if (IsXMAS(input, row, col, rows, cols))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool IsXMAS(char[][] grid, int centerRow, int centerCol, int rows, int cols)
    {
        // Early exit if center is not 'A'
        if (grid[centerRow][centerCol] != 'A')
        {
            return false;
        }

        return CheckDiagonalPair(grid, centerRow, centerCol);
    }

    private bool CheckDiagonalPair(char[][] grid, int centerRow, int centerCol)
    {
        // Get the characters for both diagonals
        char[] upLeftDiagonal = new char[3];
        char[] upRightDiagonal = new char[3];

        for (int i = 0; i < 3; i++)
        {
            upLeftDiagonal[i] = grid[centerRow + (i - 1)][centerCol + (i - 1)];
            upRightDiagonal[i] = grid[centerRow + (i - 1)][centerCol - (i - 1)];
        }

        return (IsMAS(upLeftDiagonal) && IsMAS(upRightDiagonal)) || 
               (IsMAS(upLeftDiagonal) && IsSAM(upRightDiagonal)) ||
               (IsSAM(upLeftDiagonal) && IsMAS(upRightDiagonal)) ||
               (IsSAM(upLeftDiagonal) && IsSAM(upRightDiagonal));
    }

    private bool IsMAS(char[] chars) => chars[0] == 'M' && chars[1] == 'A' && chars[2] == 'S';
    private bool IsSAM(char[] chars) => chars[0] == 'S' && chars[1] == 'A' && chars[2] == 'M';

    private bool IsInBounds(int row, int col, int rows, int cols)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }
}