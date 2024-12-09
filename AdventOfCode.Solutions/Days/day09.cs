using AdventOfCode.Solutions.Common;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2024
{
    public class Day09 : BaseDay<int[]>
    {
        protected override int DayNumber => 9;
        
        protected override int[] Parse(string[] input)
        {
            string concatenated = string.Join("", input);
            return concatenated.Select(c => int.Parse(c.ToString())).ToArray();
        }
        
        protected override object Solve1(int[] input)
        {
            var disk = BuildDisk(input);

            // Compact files by moving rightmost file blocks into leftmost free space
            bool madeMove;
            do
            {
                madeMove = false;
                int left = 0;
                int right = disk.Count - 1;

                // Find the leftmost null (free space)
                while (left < disk.Count && disk[left] != null)
                    left++;

                // Find the rightmost file block
                while (right > left && disk[right] == null)
                    right--;

                // Move a block if possible
                if (left < right && disk[left] == null && disk[right] != null)
                {
                    disk[left] = disk[right];
                    disk[right] = null;
                    madeMove = true;
                }
            } while (madeMove);

            long checksum = CalculateChecksum(disk);
            return checksum;
        }

        protected override object Solve2(int[] input)
        {
            // Build the initial disk layout
            var disk = BuildDisk(input);

            // Identify files and their positions
            // We'll store for each file ID: (startIndex, length)
            // We'll find these by scanning the disk
            var filePositions = new Dictionary<int, (int start, int length)>();
            {
                int currentFileID = -1;
                int currentFileStart = -1;
                int currentFileLength = 0;

                for (int i = 0; i < disk.Count; i++)
                {
                    if (disk[i].HasValue)
                    {
                        int fid = disk[i].Value;
                        if (fid != currentFileID)
                        {
                            // We encountered a new file block
                            if (currentFileID != -1)
                            {
                                // Save previous file data
                                filePositions[currentFileID] = (currentFileStart, currentFileLength);
                            }
                            currentFileID = fid;
                            currentFileStart = i;
                            currentFileLength = 1;
                        }
                        else
                        {
                            currentFileLength++;
                        }
                    }
                    else
                    {
                        // If we were tracking a file, save it and reset
                        if (currentFileID != -1)
                        {
                            filePositions[currentFileID] = (currentFileStart, currentFileLength);
                            currentFileID = -1;
                            currentFileStart = -1;
                            currentFileLength = 0;
                        }
                    }
                }
                // If ended while still tracking a file
                if (currentFileID != -1)
                {
                    filePositions[currentFileID] = (currentFileStart, currentFileLength);
                }
            }

            // Move files in order of decreasing file ID
            int maxFileID = filePositions.Keys.Max();
            for (int fid = maxFileID; fid >= 0; fid--)
            {
                var (start, length) = filePositions[fid];
                
                // Find a contiguous free space span to the left of 'start' that can hold this entire file
                // look for free spaces strictly at indices < start
                // The leftmost suitable span should be chosen
                // Strategy: scan from left up to 'start-1' and find largest free spaces
                int bestStart = -1;
                for (int i = 0; i < start; i++)
                {
                    if (disk[i] == null)
                    {
                        // found a free block, check how long this free run is
                        int runLength = 0;
                        int j = i;
                        while (j < start && j < disk.Count && disk[j] == null && runLength < length)
                        {
                            runLength++;
                            j++;
                        }

                        if (runLength == length)
                        {
                            // Found a suitable span
                            bestStart = i;
                            break; // This is the leftmost suitable span
                        }

                        // Skip past the run we just tested
                        i = j;
                    }
                }

                if (bestStart != -1)
                {
                    // Move the entire file to [bestStart, bestStart + length - 1]
                    // First, clear old location
                    for (int i = start; i < start + length; i++)
                    {
                        disk[i] = null;
                    }

                    // Place file in new location
                    for (int i = 0; i < length; i++)
                    {
                        disk[bestStart + i] = fid;
                    }

                    // Update the filePositions
                    filePositions[fid] = (bestStart, length);
                }
            }

            // Calculate checksum after rearrangement
            long checksum = CalculateChecksum(disk);
            return checksum;
        }

        private static List<int?> BuildDisk(int[] input)
        {
            var disk = new List<int?>();
            int fileID = 0;

            int i = 0;
            for (; i < input.Length - 1; i += 2)
            {
                int fileLength = input[i];
                int freeLength = input[i + 1];

                // Add file blocks
                disk.AddRange(Enumerable.Repeat<int?>(fileID, fileLength));
                // Add free space
                disk.AddRange(Enumerable.Repeat<int?>(null, freeLength));
                fileID++;
            }

            // If odd number of inputs, last is a file length with no trailing space
            if (input.Length % 2 != 0)
            {
                int fileLength = input[input.Length - 1];
                disk.AddRange(Enumerable.Repeat<int?>(fileID, fileLength));
                fileID++;
            }

            return disk;
        }

        private static long CalculateChecksum(List<int?> disk)
        {
            long checksum = 0;
            for (int pos = 0; pos < disk.Count; pos++)
            {
                if (disk[pos].HasValue)
                {
                    checksum += (long)pos * disk[pos].Value;
                }
            }
            return checksum;
        }
    }
}
