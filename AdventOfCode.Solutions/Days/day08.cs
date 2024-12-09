using AdventOfCode.Solutions.Common;

namespace AdventOfCode.Solutions.Year2024;

public class Day08 : BaseDay<char[][]>
{
    protected override int DayNumber => 8;
    
    protected override char[][] Parse(string[] input)
    {
        return input.Select(line => line.ToCharArray()).ToArray();
    }
    
    protected override object Solve1(char[][] input)
    {
        int height = input.Length;
        int width = input[0].Length;
        var antinodes = new HashSet<(int row, int col)>();
        
        var antennasByFreq = new Dictionary<char, List<(int row, int col)>>();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char cell = input[row][col];
                if (cell != '.')
                {
                    if (!antennasByFreq.ContainsKey(cell))
                        antennasByFreq[cell] = new List<(int row, int col)>();
                    antennasByFreq[cell].Add((row, col));
                }
            }
        }

        // For each frequency, find pairs and their antinodes
        foreach (var antennas in antennasByFreq.Values)
        {
            for (int i = 0; i < antennas.Count; i++)
            {
                for (int j = i + 1; j < antennas.Count; j++)
                {
                    var (r1, c1) = antennas[i];
                    var (r2, c2) = antennas[j];
                    
                    // Actually drawing out the vectors made this so much more tractable
                    // it also made the wording of the question make much more sense
                    // for A and B antinodes are 2A - B and 2B - A
                    var antinode1 = (row: 2 * r1 - r2, col: 2 * c1 - c2);
                    var antinode2 = (row: 2 * r2 - r1, col: 2 * c2 - c1);

                    if (IsInBounds(antinode1.row, antinode1.col, height, width))
                        antinodes.Add(antinode1);
                    if (IsInBounds(antinode2.row, antinode2.col, height, width))
                        antinodes.Add(antinode2);
                }
            }
        }

        return antinodes.Count;
    }
    
    // Very grateful to just copy/paste this method
    private bool IsInBounds(int row, int col, int height, int width)
    {
        return row >= 0 && row < height && col >= 0 && col < width;
    }

    protected override object Solve2(char[][] input)
    {
        int height = input.Length;
        int width = input[0].Length;
        var antinodes = new HashSet<(int row, int col)>();
        
        var antennasByFreq = new Dictionary<char, List<(int row, int col)>>();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char cell = input[row][col];
                if (cell != '.')
                {
                    if (!antennasByFreq.ContainsKey(cell))
                        antennasByFreq[cell] = new List<(int row, int col)>();
                    antennasByFreq[cell].Add((row, col));
                }
            }
        }

        // For each frequency, find pairs and their antinodes
        foreach (var antennas in antennasByFreq.Values)
        {
            for (int i = 0; i < antennas.Count; i++)
            {
                for (int j = i + 1; j < antennas.Count; j++)
                {
                    var (r1, c1) = antennas[i];
                    var (r2, c2) = antennas[j];
                    
                    var delta = (row: r1 - r2, col: c1 - c2);
                    // I really should break out of this early
                    for (int k = 0; k < Math.Max(height,width); k++)
                    {
                        var antinode1 = (row: r1 + (k* delta.row), col: c1 +(k * delta.col));
                        var antinode2  = (row: r1 - (k* delta.row), col: c1 -(k * delta.col));
                        
                        if (IsInBounds(antinode1.row, antinode1.col, height, width))
                            antinodes.Add(antinode1);
                        if (IsInBounds(antinode2.row, antinode2.col, height, width))
                            antinodes.Add(antinode2);
                    }
                }
            }
        }

        return antinodes.Count;
    }
}