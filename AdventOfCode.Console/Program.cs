using AdventOfCode.Solutions.Days;

var day = new Day03();
Console.WriteLine(day.Solve1());
Console.WriteLine(day.Solve2());

/**** HINTS ****
data.Select(v => Convert.ToInt32(v)) // Convert to int
data.Split("\r\n\r\n");              // Split black lines
data.Select((x, i) => new { x, i })  // Project with index
data.GroupBy(x => x);                // Group by each unique
data.GroupBy(x => x).Select(g=> new {Key = g.Key.ToString(), Tally = g.Count()}).OrderByDescending(stat=>stat.Tally).ToList(); // Histogram


first.Intersect(second)               // Set AND
first.Except(second)                  // EXCLUDE
first.Union(second)                   // UNION (does not create duplicates)
first.Concat(second)                  // Concatenation (creates duplications) 
first.Distinct(second)                // UNIQUE

*/

