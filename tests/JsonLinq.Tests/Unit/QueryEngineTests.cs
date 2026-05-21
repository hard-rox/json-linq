using JsonLinq.Core;

namespace JsonLinq.Tests.Unit;

public sealed class QueryEngineTests
{
    [Fact]
    public void Filter_UsesInjectedMatcher()
    {
        var matcher = NSubstitute.Substitute.For<IMatcher>();
        matcher.IsMatch(Arg.Any<JsonNode?>(), Arg.Any<JsonCondition>()).Returns(true);

        var engine = new QueryEngine(matcher);
        var source = new[] { JsonNode.Parse("{\"name\":\"A\"}"), JsonNode.Parse("{\"name\":\"B\"}") };

        var result = engine.Filter(source, new JsonCondition("name", "==", "A"));

        Assert.Equal(2, result.Count);
        matcher.Received(2).IsMatch(Arg.Any<JsonNode?>(), Arg.Any<JsonCondition>());
    }
}
