using JsonLinq.Examples.Scenarios;

string dataPath = Path.Combine(AppContext.BaseDirectory, "data.json");
if (!File.Exists(dataPath))
{
	dataPath = Path.Combine(Directory.GetCurrentDirectory(), "data.json");
}

string json = File.ReadAllText(dataPath);

BasicFiltering.Run(json);
Aggregations.Run(json);
Grouping.Run(json);
Sorting.Run(json);
