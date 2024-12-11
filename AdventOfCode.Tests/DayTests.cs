using AdventOfCode.Solutions.Common;
using AdventOfCode.Solutions.Days;

namespace AdventOfCode.Tests;

public class DayTests
{
    [TestCase(typeof(Day01), "1110981", "24869388")]
    [TestCase(typeof(Day02), "220", "296")]
    [TestCase(typeof(Day03), "181345830", "98729041")]
    public void Testing(Type dayType, string expectedPart1, string expectedPart2)
    {
        BaseDay? dayObj = Activator.CreateInstance(dayType) as BaseDay;

        if (!string.IsNullOrEmpty(expectedPart1))
        {
            Assert.That(dayObj!.Solve1(), Is.EqualTo(expectedPart1));
        }

        if (!string.IsNullOrEmpty(expectedPart2))
        {
            Assert.That(dayObj!.Solve2(), Is.EqualTo(expectedPart2));
        }
    }
}
