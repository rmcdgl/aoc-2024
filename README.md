# AdventOfCode

C# solutions to the [Advent Of Code 2024](https://adventofcode.com/) Puzzles.

- Unit Tests are provided to verify the answers for Part 1 and Part 2 of the puzzles. These Tests are run automatically when code is pushed to the `main` branch using GitHub Actions.
- The convenience script `Get-AdventOfCodeInput.ps1` can be used to download puzzle inputs, using the Advent of Code API. 
- Because the script is not digitally signed, you must bypass Windows security policy using

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
```

## Performance and Memory Benchmarking

To measure the run duration and memory usage, set the Start up project to be `AdventOfCode.Benchmark`, rebuild the projects in `Release` mode, and run the Benchmark project. This will output timing and memory usage using BenchmarkDotnet.

----
The structure of this solution was based on [this example](https://github.com/nick-wilson95/AdventOfCode2022)