using TrelloDotNet.Model;

namespace TrelloDotNet.Tests.UnitTests;

public class ChecklistExtensionsTests
{
    [Fact]
    public void ChecklistCountsItemsByState()
    {
        Checklist checklist = new Checklist("Checklist",
        [
            new ChecklistItem("Done") { State = ChecklistItemState.Complete },
            new ChecklistItem("Todo") { State = ChecklistItemState.Incomplete },
            new ChecklistItem("Also Done") { State = ChecklistItemState.Complete }
        ]);

        Assert.Equal(3, checklist.GetNumberOfItems());
        Assert.Equal(2, checklist.GetNumberOfCompletedItems());
        Assert.Equal(1, checklist.GetNumberOfIncompleteItems());
        Assert.False(checklist.IsAllComplete());
        Assert.True(checklist.IsAnyIncomplete());
    }

    [Fact]
    public void ChecklistCollectionAggregatesCountsAndCompletion()
    {
        List<Checklist> checklists =
        [
            new Checklist("Complete",
            [
                new ChecklistItem("Done") { State = ChecklistItemState.Complete }
            ]),
            new Checklist("Mixed",
            [
                new ChecklistItem("Done") { State = ChecklistItemState.Complete },
                new ChecklistItem("Todo") { State = ChecklistItemState.Incomplete }
            ])
        ];

        Assert.Equal(3, checklists.GetNumberOfItems());
        Assert.Equal(2, checklists.GetNumberOfCompletedItems());
        Assert.Equal(1, checklists.GetNumberOfIncompleteItems());
        Assert.False(checklists.IsAllComplete());
        Assert.True(checklists.IsAnyIncomplete());
    }

    [Fact]
    public void EmptyChecklistIsAllCompleteAndHasNoIncompleteItems()
    {
        Checklist checklist = new Checklist("Empty", []);

        Assert.True(checklist.IsAllComplete());
        Assert.False(checklist.IsAnyIncomplete());
    }
}
