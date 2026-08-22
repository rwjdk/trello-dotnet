using System.Reflection;
using TrelloDotNet.AutomationEngine;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.Tests.AutomationEngineTests.SampleActions;
using TrelloDotNet.Tests.AutomationEngineTests.SampleConditions;
using TrelloDotNet.Tests.AutomationEngineTests.SampleTriggers;

namespace TrelloDotNet.Tests.AutomationEngineTests;

public class BasicFlowTests : TestBase
{
    [Fact]
    public async Task TrueTrigger()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(2, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
        Assert.NotNull(result.Log);
        Assert.NotNull(result.Log[0].Message);
        Assert.NotEmpty(result.Log[0].Timestamp.ToString());
    }

    [Fact]
    public async Task TrueTriggersTrueFalse()
    {
        AlwaysTrueTrigger trueTrigger = new AlwaysTrueTrigger();
        AlwaysFalseTrigger falseTrigger = new AlwaysFalseTrigger();
        AutomationController automationController = CreateAutomationController([trueTrigger, falseTrigger], [], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(2, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
        Assert.NotNull(result.Log);
        Assert.NotNull(result.Log[0].Message);
        Assert.NotEmpty(result.Log[0].Timestamp.ToString());
    }

    [Fact]
    public async Task TrueTriggersFalseTrue()
    {
        AlwaysFalseTrigger falseTrigger = new AlwaysFalseTrigger();
        AlwaysTrueTrigger trueTrigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController([falseTrigger, trueTrigger], [], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(2, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
        Assert.NotNull(result.Log);
        Assert.NotNull(result.Log[0].Message);
        Assert.NotEmpty(result.Log[0].Timestamp.ToString());
    }

    [Fact]
    public async Task SkipAction()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [], [new SkipAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(0, result.ActionsExecuted);
        Assert.Equal(2, result.ActionsSkipped);
    }

    [Fact]
    public async Task TrueTriggerFalseCondition()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new AlwaysFalseCondition()], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(0, result.AutomationsProcessed);
        Assert.Equal(2, result.AutomationsSkipped);
        Assert.Equal(0, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task TrueTriggerTrueCondition()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new AlwaysTrueCondition()], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(2, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task TrueTrigger2TrueConditions()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new AlwaysTrueCondition(), new AlwaysTrueCondition()], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(2, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task TrueTrigger2ConditionsOneTrueAndOneFalse()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new AlwaysTrueCondition(), new AlwaysFalseCondition()], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(0, result.AutomationsProcessed);
        Assert.Equal(2, result.AutomationsSkipped);
        Assert.Equal(0, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task FalseTrigger()
    {
        AlwaysFalseTrigger triggeer = new AlwaysFalseTrigger();
        AutomationController automationController = CreateAutomationController(triggeer, [], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(0, result.AutomationsProcessed);
        Assert.Equal(2, result.AutomationsSkipped);
        Assert.Equal(0, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task FalseTriggers()
    {
        AlwaysFalseTrigger falseTrigger1 = new AlwaysFalseTrigger();
        AlwaysFalseTrigger falseTrigger2 = new AlwaysFalseTrigger();
        AutomationController automationController = CreateAutomationController([falseTrigger1, falseTrigger2], [], [new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(0, result.AutomationsProcessed);
        Assert.Equal(2, result.AutomationsSkipped);
        Assert.Equal(0, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task TrueTriggerAndCondition()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new AlwaysTrueCondition()], [new DummyAction(), new DummyAction()]);
        ProcessingResult result = await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json")));
        Assert.Equal(2, result.AutomationsProcessed);
        Assert.Equal(0, result.AutomationsSkipped);
        Assert.Equal(4, result.ActionsExecuted);
        Assert.Equal(0, result.ActionsSkipped);
    }

    [Fact]
    public async Task ExceptionTrigger()
    {
        ExceptionTrigger trigger = new ExceptionTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [], [new ExceptionAction()]);
        await Assert.ThrowsAsync<AutomationException>(async () => await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json"))));
    }

    [Fact]
    public async Task ExceptionCondition()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [new ExceptionCondition()], [new ExceptionAction()]);
        await Assert.ThrowsAsync<AutomationException>(async () => await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json"))));
    }

    [Fact]
    public async Task ExceptionAction()
    {
        AlwaysTrueTrigger trigger = new AlwaysTrueTrigger();
        AutomationController automationController = CreateAutomationController(trigger, [], [new ExceptionAction()]);
        await Assert.ThrowsAsync<AutomationException>(async () => await automationController.ProcessJsonFromWebhookAsync(new ProcessingRequest(GetJsonFromSampleFile("MoveCardFromListToList.json"))));
    }


    private AutomationController CreateAutomationController(IAutomationTrigger trigger, List<IAutomationCondition> conditions, List<IAutomationAction> actions)
    {
        const string name = "X";
        Automation automation1 = new Automation(name, trigger, conditions, actions);
        Automation automation2 = new Automation(name, trigger, conditions, actions);
        Configuration configuration = new Configuration(TrelloClient, automation1, automation2);
        AutomationController automationController = new AutomationController(configuration);
        return automationController;
    }

    private AutomationController CreateAutomationController(List<IAutomationTrigger> triggers, List<IAutomationCondition> conditions, List<IAutomationAction> actions)
    {
        const string name = "X";
        Automation automation1 = new Automation(name, triggers, conditions, actions);
        Automation automation2 = new Automation(name, triggers, conditions, actions);
        Configuration configuration = new Configuration(TrelloClient, automation1, automation2);
        AutomationController automationController = new AutomationController(configuration);
        return automationController;
    }

    private string GetJsonFromSampleFile(string filename)
    {
        const string sampleJsonFolder = "SampleJson";
        const string webhookEventsFolder = "WebhookEvents";
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        string path = Path.GetDirectoryName(assemblyLocation) +
                      Path.DirectorySeparatorChar +
                      sampleJsonFolder +
                      Path.DirectorySeparatorChar +
                      webhookEventsFolder +
                      Path.DirectorySeparatorChar +
                      filename;
        return File.ReadAllText(path);
    }
}