using JsonLinq.Examples.Scenarios;

var dataPath = Path.Combine(AppContext.BaseDirectory, "data.json");
if (!File.Exists(dataPath))
{
	dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
}

var json = File.ReadAllText(dataPath);

BasicFiltering.Run(json);
Aggregations.Run(json);
Grouping.Run(json);
Sorting.Run(json);
