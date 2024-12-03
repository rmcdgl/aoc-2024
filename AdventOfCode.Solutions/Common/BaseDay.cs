using BenchmarkDotNet.Attributes;

namespace AdventOfCode.Solutions.Common;

public abstract class BaseDay
{
    public abstract string? Solve1();
    public abstract string? Solve2();
}

public abstract class BaseDay<TInput> : BaseDay
{
    const int Year = 2024;

    protected abstract int DayNumber { get; }

    protected abstract TInput Parse(string[] input);

    private string[] _input;

    protected BaseDay()
    {
        _input = File.ReadAllLines($@"Inputs/{DayNumber:00}.txt")
            .ToArray();
    }

    [Benchmark]
    public override string? Solve1() => Solve1(Parse(_input)).ToString();

    protected abstract object Solve1(TInput input);

    [Benchmark]
    public override string? Solve2() => Solve2(Parse(_input)).ToString();

    protected abstract object Solve2(TInput input);
}