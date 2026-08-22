using TrelloDotNet.AutomationEngine;
using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.Tests.AutomationEngineTests.SampleTriggers;

namespace TrelloDotNet.Tests.AutomationEngineTests;

public class GuardTests : TestBase
{
    [Fact]
    public void ControllerNullConfiguration()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new AutomationController(null));
        Assert.StartsWith("You did not provide a configuration", exception.Message);
    }

    [Fact]
    public void ConfigurationNullTrelloClient()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Configuration(null));
        Assert.StartsWith("TrelloClient can't be null", exception.Message);
    }

    [Fact]
    public void ConfigurationNoAutomations()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Configuration(TrelloClient));
        Assert.StartsWith("You need to provide at least one Automation", exception.Message);
    }

    [Fact]
    public void AutomationNullTrigger()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Automation(null, trigger: null, null, null));
        Assert.StartsWith("Trigger can't be null", exception.Message);
    }

    [Fact]
    public void AutomationNullTriggers()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Automation(null, triggers: null, null, null));
        Assert.StartsWith("Trigger can't be null", exception.Message);
    }

    [Fact]
    public void AutomationNoTriggers()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Automation(null, triggers: [], null, null));
        Assert.StartsWith("You need at least one Trigger", exception.Message);
    }

    [Fact]
    public void AutomationNullAction()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new Automation(null, new AlwaysTrueTrigger(), null, null));
        Assert.StartsWith("Actions can't be null", exception.Message);
    }

    [Fact]
    public void AutomationNoActions()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new Automation(null, new AlwaysTrueTrigger(), null, []));
        Assert.StartsWith("You need at least one action", exception.Message);
    }
}