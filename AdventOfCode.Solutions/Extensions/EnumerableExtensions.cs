namespace AdventOfCode.Solutions.Extensions;

public static class EnumerableExtensions
{
    public static List<List<T>> Split<T>(this IEnumerable<T> input, Func<T, bool> splitOn)
    {
        var result = new List<List<T>> { new() };

        input.ToList().ForEach(x =>
        {
            if (splitOn(x))
            {
                result.Add(new List<T>());
            }
            else
            {
                result.Last().Add(x);
            }
        });

        return result;
    }

    public static IEnumerable<T> Dump<T>(this IEnumerable<T> input)
    {
        foreach (var element in input)
        {
            Console.WriteLine(element);
            yield return element;
        }
    }

    public static IEnumerable<string> SkipEmptyLines(this IEnumerable<string> input)
    {
        foreach (var element in input)
        {
            if (!string.IsNullOrWhiteSpace(element))
                yield return element;
        }
    }
    public static List<List<long>> ConvertToLong(this IEnumerable<string> input)
    {
        return input.Select(x => x.Split(' ').Select(v => Convert.ToInt64(v)).ToList()).ToList();
    }
}