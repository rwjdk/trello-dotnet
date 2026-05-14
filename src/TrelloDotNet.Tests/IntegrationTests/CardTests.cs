using TrelloDotNet.Model;
using TrelloDotNet.Model.Actions;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.AddCardFromTemplateOptions;
using TrelloDotNet.Model.Options.AddCardOptions;
using TrelloDotNet.Model.Options.CopyCardOptions;
using TrelloDotNet.Model.Options.DueRecurrenceOptions;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Options.MirrorCardOptions;
using TrelloDotNet.Model.Options.MoveCardToBoardOptions;
using TrelloDotNet.Model.Options.MoveCardToListOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.Tests.IntegrationTests;

public class CardTests(TestFixtureWithNewBoard fixture) : TestBase, IClassFixture<TestFixtureWithNewBoard>
{
    private readonly Board _board = fixture.Board!;

    [Fact]
    public async Task Attachments()
    {
        Card card = await AddDummyCard(_board.Id, "Attachments");
        Attachment att1 = await TrelloClient.AddAttachmentToCardAsync(card.Id, new AttachmentUrlLink("https://www.rwj.dk", "Some webpage URL"), cancellationToken: TestCancellationToken);

        Attachment att2;
        try
        {
            att2 = await AddImageAttachment(card);
        }
        catch
        {
            try
            {
                await Task.Delay(1000, TestCancellationToken);
                att2 = await AddImageAttachment(card);
            }
            catch
            {
                await Task.Delay(1000, TestCancellationToken);
                att2 = await AddImageAttachment(card);
            }
        }

        Card cardAfterAttachments = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            IncludeAttachments = GetCardOptionsIncludeAttachments.True
        }, cancellationToken: TestCancellationToken);
        Assert.Equal(2, cardAfterAttachments.Attachments.Count);
        Assert.Equal(att2.Id, cardAfterAttachments.AttachmentCover);

        List<Attachment>? attachments = await TrelloClient.GetAttachmentsOnCardAsync(card.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(2, attachments.Count);
        Attachment attachment1 = attachments.Single(x => x.Id == att1.Id);
        Attachment attachment2 = attachments.Single(x => x.Id == att2.Id);

        Stream stream = await TrelloClient.DownloadAttachmentAsync(card.Id, attachment1.Id, cancellationToken: TestCancellationToken);
        Assert.NotNull(stream);

        Assert.Equal(att1.Url, attachment1.Url);
        Assert.Equal(att1.Name, attachment1.Name);
        Assert.Equal(att1.IsUpload, attachment1.IsUpload);
        Assert.Equal(att1.EdgeColor, attachment1.EdgeColor);
        Assert.Equal(att1.Bytes, attachment1.Bytes);
        Assert.Equal(att1.Position, attachment1.Position);
        Assert.Equal(att1.Created, attachment1.Created);
        Assert.Equal(att1.MemberId, attachment1.MemberId);
        Assert.Equal(att1.MimeType, attachment1.MimeType);
        Assert.Equal(att1.Date, attachment1.Date);


        Assert.Equal(att2.FileName, attachment2.FileName);
        Assert.Equal(att2.Name, attachment2.Name);

        await TrelloClient.DeleteAttachmentOnCardAsync(card.Id, attachment1.Id, cancellationToken: TestCancellationToken);
        List<Attachment>? attachmentsAfterDelete = await TrelloClient.GetAttachmentsOnCardAsync(card.Id, cancellationToken: TestCancellationToken);
        Assert.Single(attachmentsAfterDelete);
        attachment2 = attachmentsAfterDelete.Single(x => x.Id == att2.Id);
        Assert.Equal(att2.FileName, attachment2.FileName);
        Assert.Equal(att2.Name, attachment2.Name);
    }

    private async Task<Attachment> AddImageAttachment(Card card)
    {
        await using Stream stream = File.Open("TestData" + Path.DirectorySeparatorChar + "TestImage.png", FileMode.Open);
        AttachmentFileUpload attachmentFileUpload = new AttachmentFileUpload(stream, "MyFileName.png", "SomeName");
        Attachment att2 = await TrelloClient.AddAttachmentToCardAsync(card.Id, attachmentFileUpload, true, cancellationToken: TestCancellationToken);
        return att2;
    }

    [Fact]
    public async Task AttachmentNamedPositions()
    {
        Card card = await AddDummyCard(_board.Id, "AttachmentNamedPositions");
        AttachmentUrlLink topAttachment = new AttachmentUrlLink("https://www.rwj.dk/top", "Top Attachment")
        {
            NamedPosition = NamedPosition.Top
        };
        AttachmentUrlLink bottomAttachment = new AttachmentUrlLink("https://www.rwj.dk/bottom", "Bottom Attachment")
        {
            NamedPosition = NamedPosition.Bottom
        };

        Attachment addedTopAttachment = await TrelloClient.AddAttachmentToCardAsync(card.Id, topAttachment, cancellationToken: TestCancellationToken);
        Attachment addedBottomAttachment = await TrelloClient.AddAttachmentToCardAsync(card.Id, bottomAttachment, cancellationToken: TestCancellationToken);

        Attachment retrievedTopAttachment = await TrelloClient.GetAttachmentOnCardAsync(card.Id, addedTopAttachment.Id, cancellationToken: TestCancellationToken);
        Attachment retrievedBottomAttachment = await TrelloClient.GetAttachmentOnCardAsync(card.Id, addedBottomAttachment.Id, cancellationToken: TestCancellationToken);
        List<Attachment> attachments = await TrelloClient.GetAttachmentsOnCardAsync(card.Id, cancellationToken: TestCancellationToken);

        Assert.Equal("Top Attachment", retrievedTopAttachment.Name);
        Assert.Equal("Bottom Attachment", retrievedBottomAttachment.Name);
        Assert.Contains(attachments, x => x.Id == addedTopAttachment.Id);
        Assert.Contains(attachments, x => x.Id == addedBottomAttachment.Id);
    }

    [Fact]
    public async Task GetCard()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        AddCardOptions inputCard = new AddCardOptions(list.Id, Guid.NewGuid().ToString())
        {
            Due = DateTimeOffset.Now,
            Description = Guid.NewGuid().ToString()
        };
        Card addCard = await TrelloClient.AddCardAsync(inputCard, cancellationToken: TestCancellationToken);
        Card? getCard = await TrelloClient.GetCardAsync(addCard.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(addCard.Description, getCard.Description);
        Assert.Equal(addCard.DueComplete, getCard.DueComplete);
    }

    [Fact]
    public async Task AddCard()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<string> memberIds = new List<string> { member.Id };
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        DateTimeOffset? start = new DateTimeOffset(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        DateTimeOffset? due = new DateTimeOffset(new DateTime(2099, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        //Add Card
        List<string> labelIds = new List<string>
        {
            allLabelsOnBoard[1].Id,
            allLabelsOnBoard[2].Id
        };
        AddCardOptions input = new AddCardOptions(list.Id, "Card", "Description")
        {
            Start = start,
            Due = due,
            DueComplete = false,
            LabelIds = labelIds,
            MemberIds = memberIds,
            Cover = new CardCover(CardCoverColor.Blue, CardCoverSize.Normal)
        };
        Card? addedCard = await TrelloClient.AddCardAsync(input, cancellationToken: TestCancellationToken);
        AssertTimeIsNow(addedCard.Created);
        AssertTimeIsNow(addedCard.LastActivity);
        Assert.Equal("Card", addedCard.Name);
        Assert.Equal("Description", addedCard.Description);
        Assert.Equal(start, addedCard.Start);
        Assert.Equal(due, addedCard.Due);
        Assert.NotNull(addedCard.Cover);
        Assert.Equal(CardCoverColor.Blue, addedCard.Cover.Color);
        Assert.Equal(CardCoverSize.Normal, addedCard.Cover.Size);
        Assert.False(addedCard.DueComplete);
        Assert.Equal(2, addedCard.Labels.Count);
        Assert.Equal(allLabelsOnBoard[1].Color, addedCard.Labels[0].Color);
        Assert.Equal(allLabelsOnBoard[2].Color, addedCard.Labels[1].Color);
        Assert.Single(addedCard.MemberIds);
        Assert.Single(await TrelloClient.GetCardsInListAsync(list.Id, cancellationToken: TestCancellationToken));
        Assert.NotEmpty(addedCard.Url);
        Assert.NotEmpty(addedCard.ShortUrl);
        Assert.NotEmpty(addedCard.IdShort.ToString());
        Assert.NotNull(addedCard.Cover);
        Assert.Equal(CardCoverSize.Normal, addedCard.Cover.Size);
        Assert.Equal(CardCoverBrightness.Dark, addedCard.Cover.Brightness);
        Assert.Null(addedCard.Cover.BackgroundImageId);
        Assert.Empty(addedCard.ChecklistIds);

        List<Member>? membersOfCardAsync = await TrelloClient.GetMembersOfCardAsync(addedCard.Id, cancellationToken: TestCancellationToken);
        Assert.Single(membersOfCardAsync);
    }

    [Fact]
    public async Task AddCardValidation()
    {
        ArgumentNullException nullOptionsException = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await TrelloClient.AddCardAsync(null!, cancellationToken: TestCancellationToken));
        Assert.Equal("options", nullOptionsException.ParamName);

        TrelloApiException missingListIdException = await Assert.ThrowsAsync<TrelloApiException>(async () =>
            await TrelloClient.AddCardAsync(new AddCardOptions
            {
                Name = "Missing ListId"
            }, cancellationToken: TestCancellationToken));
        Assert.Equal("No ListId provided in options (Mandatory)", missingListIdException.Message);
    }

    [Fact]
    public async Task AddCardWithExplicitPosition()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);

        Card addedCard = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Positioned Card")
        {
            Position = 1234
        }, cancellationToken: TestCancellationToken);

        Assert.Equal("Positioned Card", addedCard.Name);
        Assert.Equal(list.Id, addedCard.ListId);
        Assert.Equal(1234, addedCard.Position);
    }

    [Fact]
    public async Task AddCardFull()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        await using Stream stream = File.Open("TestData" + Path.DirectorySeparatorChar + "TestImage.png", FileMode.Open);
        await TrelloClient.AddCardAsync(new AddCardOptions
        {
            //Mandatory options
            ListId = list.Id,
            Name = "My Card",

            //Optional options
            Description = "Description of My Card",
            Start = DateTimeOffset.Now,
            Due = DateTimeOffset.Now.AddDays(3),
            Cover = new CardCover(CardCoverColor.Blue, CardCoverSize.Normal),
            LabelIds = allLabelsOnBoard.Select(x => x.Id).ToList(),
            MemberIds = [member.Id],
            Checklists =
            [
                new Checklist("Checklist 1", [
                    new ChecklistItem("Item 1"),
                    new ChecklistItem("Item 2"),
                    new ChecklistItem("Item 3")
                ]),

                new Checklist("Checklist 2", [
                    new ChecklistItem("Item A"),
                    new ChecklistItem("Item B"),
                    new ChecklistItem("Item C")
                ])
            ],
            AttachmentUrlLinks =
            [
                new AttachmentUrlLink("https://www.google.com", "Google")
            ],
            AttachmentFileUploads =
            [
                new AttachmentFileUpload(stream, "X", "Z")
            ]
        }, cancellationToken: TestCancellationToken);
    }

    [Fact]
    public async Task UpdateCard()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Card addedCard = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "X", "X"), cancellationToken: TestCancellationToken);
        Card? updateCard = await TrelloClient.UpdateCardAsync(addedCard.Id, [
            CardUpdate.DueComplete(true),
            CardUpdate.Description("New Description"),
            CardUpdate.Name("NewName"),
            CardUpdate.Closed(true),
            CardUpdate.Cover(new CardCover(CardCoverColor.Black, CardCoverSize.Full)),
            CardUpdate.DueDate(DateTimeOffset.UtcNow),
            CardUpdate.StartDate(DateTimeOffset.UtcNow),
            CardUpdate.Position(NamedPosition.Bottom),
            CardUpdate.Members(new List<string>()),
            CardUpdate.Labels(new List<string>()),
            CardUpdate.Board(_board.Id),
            CardUpdate.List(list.Id),
            CardUpdate.IsTemplate(false),
        ], cancellationToken: TestCancellationToken);
        Assert.True(updateCard.DueComplete);
        Assert.Equal("New Description", updateCard.Description);
        updateCard = await TrelloClient.UpdateCardAsync(addedCard.Id, [
            CardUpdate.DueComplete(false),
            CardUpdate.Description("New Description"),
            CardUpdate.Name("NewName"),
            CardUpdate.Closed(false),
            CardUpdate.Cover(new CardCover(CardCoverColor.Black, CardCoverSize.Full)),
            CardUpdate.DueDate(DateTimeOffset.UtcNow),
            CardUpdate.StartDate(DateTimeOffset.UtcNow),
            CardUpdate.Position(1),
            CardUpdate.Members(new List<Member>()),
            CardUpdate.Labels(new List<Label>()),
            CardUpdate.Board(_board),
            CardUpdate.List(list),
            CardUpdate.IsTemplate(true),
        ], cancellationToken: TestCancellationToken);
        Assert.False(updateCard.DueComplete);
        Assert.False(updateCard.Closed);
        Assert.Equal(1, updateCard.Position);
    }

    [Fact]
    public async Task CopyCard()
    {
        (List list, Card card) = await AddDummyCardAndList(_board.Id);
        Card copy = await TrelloClient.CopyCardAsync(new CopyCardOptions
        {
            Name = "NewName",
            NamedPosition = NamedPosition.Top,
            Keep = CopyCardOptionsToKeep.Due | CopyCardOptionsToKeep.Labels | CopyCardOptionsToKeep.Start,
            SourceCardId = card.Id,
            TargetListId = list.Id
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(copy.Id, card.Id);
        Assert.NotEqual(copy.Name, card.Name);
    }

    [Fact]
    public async Task CopyCardWithExplicitPositionAndAllKeep()
    {
        (List list, Card card) = await AddDummyCardAndList(_board.Id);

        Card copy = await TrelloClient.CopyCardAsync(new CopyCardOptions
        {
            Position = 1234,
            Keep = CopyCardOptionsToKeep.All,
            SourceCardId = card.Id,
            TargetListId = list.Id
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(copy.Id, card.Id);
        Assert.Equal(card.Name, copy.Name);
        Assert.Equal(list.Id, copy.ListId);
    }

    [Fact]
    public async Task AddSeparatorCards()
    {
        List list = await AddDummyList(_board.Id);

        Card separatorAtTop = await TrelloClient.AddSeparatorCardAsync(list.Id, 0M, TestCancellationToken);
        Card separatorAtBottom = await TrelloClient.AddSeparatorCardAsync(list.Id, NamedPosition.Bottom, TestCancellationToken);
        Card separatorAtPosition = await TrelloClient.AddSeparatorCardAsync(list.Id, 1234M, TestCancellationToken);

        Assert.Equal("---", separatorAtTop.Name);
        Assert.Equal("---", separatorAtBottom.Name);
        Assert.Equal("---", separatorAtPosition.Name);
        Assert.True(separatorAtTop.IsSeparator);
        Assert.True(separatorAtBottom.IsSeparator);
        Assert.True(separatorAtPosition.IsSeparator);
        Assert.Equal(list.Id, separatorAtTop.ListId);
        Assert.Equal(list.Id, separatorAtBottom.ListId);
        Assert.Equal(list.Id, separatorAtPosition.ListId);
    }

    [Fact]
    public async Task Checklists()
    {
        await using TemporaryBoardContext boardContext = await CreateTemporaryBoardAsync(nameof(Checklists));
        Board boardUnderTest = boardContext.Board;

        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", boardUnderTest.Id), TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Card"), TestCancellationToken);

        Member? member = (await TrelloClient.GetMembersOfBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken)).First();
        DateTimeOffset? due = new DateTimeOffset(new DateTime(2099, 1, 1, 12, 0, 0, DateTimeKind.Utc));

        List<Checklist>? checklists = await TrelloClient.GetChecklistsOnBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(checklists);

        List<ChecklistItem> checklistItems = new List<ChecklistItem>
        {
            new("ItemA", due, member.Id),
            new("ItemB"),
            new("ItemC", null, member.Id)
        };
        Checklist newChecklist = new Checklist("Sample Checklist", checklistItems);
        Checklist? addedChecklist = await TrelloClient.AddChecklistAsync(card.Id, newChecklist, true, cancellationToken: TestCancellationToken);
        await TrelloClient.AddChecklistAsync(card.Id, newChecklist, true, cancellationToken: TestCancellationToken);

        Assert.Equal("Sample Checklist", addedChecklist.Name);
        Assert.Equal(card.Id, addedChecklist.CardId);
        Assert.Equal(3, addedChecklist.Items.Count);
        ChecklistItem? itemA = addedChecklist.Items.FirstOrDefault(x => x.Name == "ItemA");
        ChecklistItem? itemB = addedChecklist.Items.FirstOrDefault(x => x.Name == "ItemB");
        ChecklistItem? itemC = addedChecklist.Items.FirstOrDefault(x => x.Name == "ItemC");

        Assert.NotNull(itemA);
        Assert.NotEmpty(itemA.Id);
        Assert.Equal(ChecklistItemState.Incomplete, itemA.State);
        Assert.Equal(addedChecklist.Id, itemA.ChecklistId);
        Assert.NotNull(itemB);
        Assert.NotEmpty(itemB.Id);
        Assert.Equal(addedChecklist.Id, itemB.ChecklistId);
        Assert.Equal(ChecklistItemState.Incomplete, itemB.State);
        Assert.NotNull(itemC);
        Assert.NotEmpty(itemC.Id);
        Assert.Equal(addedChecklist.Id, itemC.ChecklistId);
        Assert.Equal(ChecklistItemState.Incomplete, itemC.State);

        //Assert.Equal(due, itemA.Due); //This will fail on a free version of Trello so commented out
        //Assert.Equal(member.Id, itemA.MemberId); //This will fail on a free version of Trello so commented out

        Assert.Null(itemB.Due);
        Assert.Null(itemB.MemberId);

        Assert.Null(itemC.Due);
        //Assert.Equal(member.Id, itemC.MemberId); //This will fail on a free version of Trello so commented out

        Card? doneCard = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Card 2"), TestCancellationToken);
        await TrelloClient.AddChecklistAsync(doneCard.Id, addedChecklist.Id, true, cancellationToken: TestCancellationToken);
        await TrelloClient.AddChecklistAsync(doneCard.Id, addedChecklist.Id, true, cancellationToken: TestCancellationToken);

        //Test adding a checklist with no items
        await TrelloClient.AddChecklistAsync(doneCard.Id, new Checklist("Empty Checklist"), cancellationToken: TestCancellationToken);

        List<Checklist>? checklistsNow = await TrelloClient.GetChecklistsOnBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(3, checklistsNow.Count);

        await Task.Delay(1000, TestCancellationToken);
        Checklist checklist = checklistsNow[0];
        await TrelloClient.DeleteChecklistItemAsync(checklist.Id, checklist.Items[0].Id, TestCancellationToken);
        await Task.Delay(1000, TestCancellationToken);
        List<Checklist>? checklistsNowAfterOneDelete = await TrelloClient.GetChecklistsOnBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(checklist.Items.Count - 1, checklistsNowAfterOneDelete.First(x => x.Id == checklist.Id).Items.Count);

        await TrelloClient.DeleteChecklistAsync(addedChecklist.Id, TestCancellationToken);
        List<Checklist>? checklistsNow2 = await TrelloClient.GetChecklistsOnBoardAsync(boardUnderTest.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(2, checklistsNow2.Count);

        ChecklistItem item = await TrelloClient.AddChecklistItemAsync(checklistsNow2[0].Id, new ChecklistItem("SomeMore"), TestCancellationToken);
        Assert.Equal(checklistsNow2[0].Id, item.ChecklistId);
        Assert.Equal("SomeMore", item.Name);
        Assert.Equal(ChecklistItemState.Incomplete, item.State);

        Checklist updateChecklist = await TrelloClient.UpdateChecklistAsync(checklistsNow2[0].Id, "SomethingNew", NamedPosition.Bottom, TestCancellationToken);
        Assert.Equal("SomethingNew", updateChecklist.Name);
    }

    [Fact]
    public async Task Delete()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Card"), cancellationToken: TestCancellationToken);
        await TrelloClient.DeleteCardAsync(card.Id, cancellationToken: TestCancellationToken);
        List<Card>? cardsAfterDelete = await TrelloClient.GetCardsOnBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        Assert.True(cardsAfterDelete.All(x => x.Id != card.Id));
    }

    [Fact]
    public async Task ArchiveAndReopen()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);

        //Archive and Reopen
        Card? cardForArchiveAndReopen = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Card for archive and reopen"), cancellationToken: TestCancellationToken);
        Assert.False(cardForArchiveAndReopen.Closed);

        Card? archivedCard = await TrelloClient.ArchiveCardAsync(cardForArchiveAndReopen.Id, cancellationToken: TestCancellationToken);
        Assert.True(archivedCard.Closed);

        Card? reOpenedCard = await TrelloClient.ReOpenCardAsync(archivedCard.Id, cancellationToken: TestCancellationToken);
        Assert.False(reOpenedCard.Closed);
    }

    [Fact]
    public async Task Dates()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Test: Set dates
        Card? cardWithDates = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Date Card"), cancellationToken: TestCancellationToken);
        DateTimeOffset testStartDate = new(new DateTime(2000, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        DateTimeOffset testDueDate = testStartDate.AddDays(2);

        Card? cardWithStart = await TrelloClient.SetStartDateOnCardAsync(cardWithDates.Id, testStartDate, cancellationToken: TestCancellationToken);
        Assert.Equal(testStartDate, cardWithStart.Start);

        Card? cardWithDue = await TrelloClient.SetDueDateOnCardAsync(cardWithDates.Id, testDueDate, cancellationToken: TestCancellationToken);
        Assert.Equal(testStartDate, cardWithDue.Start);
        Assert.Equal(testDueDate, cardWithDue.Due);

        Card? cardWithStartAndDue = await TrelloClient.SetStartDateAndDueDateOnCardAsync(cardWithDates.Id, testStartDate.AddDays(1), testDueDate.AddDays(1), true, cancellationToken: TestCancellationToken);
        Assert.Equal(testStartDate.AddDays(1), cardWithStartAndDue.Start);
        Assert.Equal(testDueDate.AddDays(1), cardWithStartAndDue.Due);
    }

    [Theory]
    [MemberData(nameof(RecurringDueDateOptions))]
    public async Task SetRecurringDueDateVariations(DueRecurrenceOptions options)
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Recurring Due Date " + options.Recurrence), cancellationToken: TestCancellationToken);

        await TrelloClient.SetRecurringDueDateAsync(card.Id, options, TestCancellationToken);

        Card cardAfterSet = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Due)
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(options.FirstDueDate, cardAfterSet.Due);

        await TrelloClient.RemoveRecurringDueDateAsync(card.Id, TestCancellationToken);

        Card cardAfterRemove = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Due)
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(options.FirstDueDate, cardAfterRemove.Due);
    }

    [Fact]
    public async Task SetRecurringDueDateUsesExistingDueDateWhenFirstDueDateIsNotProvided()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        DateTimeOffset existingDueDate = new(new DateTime(2099, 2, 5, 12, 0, 0, DateTimeKind.Utc));
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Recurring Due Date Existing Due")
        {
            Due = existingDueDate
        }, cancellationToken: TestCancellationToken);

        await TrelloClient.SetRecurringDueDateAsync(card.Id, new DueRecurrenceOptions
        {
            Recurrence = DueRecurrenceType.Daily
        }, TestCancellationToken);

        Card cardAfterSet = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Due)
        }, cancellationToken: TestCancellationToken);

        Assert.Equal(existingDueDate, cardAfterSet.Due);

        await TrelloClient.RemoveRecurringDueDateAsync(card.Id, TestCancellationToken);
    }

    [Fact]
    public async Task SetRecurringDueDateSetsDueDateWhenCardHasNoDueDateAndFirstDueDateIsNotProvided()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Card card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Recurring Due Date No Due"), cancellationToken: TestCancellationToken);
        DateTimeOffset beforeSet = DateTimeOffset.UtcNow.AddMinutes(-1);

        await TrelloClient.SetRecurringDueDateAsync(card.Id, new DueRecurrenceOptions
        {
            Recurrence = DueRecurrenceType.Daily
        }, TestCancellationToken);

        Card cardAfterSet = await TrelloClient.GetCardAsync(card.Id, new GetCardOptions
        {
            CardFields = new CardFields(CardFieldsType.Due)
        }, cancellationToken: TestCancellationToken);

        Assert.NotNull(cardAfterSet.Due);
        Assert.True(cardAfterSet.Due >= beforeSet);
        Assert.True(cardAfterSet.Due <= DateTimeOffset.UtcNow.AddMinutes(1));

        await TrelloClient.RemoveRecurringDueDateAsync(card.Id, TestCancellationToken);
    }

    public static IEnumerable<object[]> RecurringDueDateOptions()
    {
        yield return
        [
            new DueRecurrenceOptions
            {
                Recurrence = DueRecurrenceType.Daily,
                FirstDueDate = new DateTimeOffset(new DateTime(2099, 1, 5, 12, 0, 0, DateTimeKind.Utc))
            }
        ];
        yield return
        [
            new DueRecurrenceOptions
            {
                Recurrence = DueRecurrenceType.Weekdays,
                FirstDueDate = new DateTimeOffset(new DateTime(2099, 1, 6, 12, 0, 0, DateTimeKind.Utc))
            }
        ];
        yield return
        [
            new DueRecurrenceOptions
            {
                Recurrence = DueRecurrenceType.Weekly,
                FirstDueDate = new DateTimeOffset(new DateTime(2099, 1, 7, 12, 0, 0, DateTimeKind.Utc)),
                Monday = true,
                Wednesday = true,
                Friday = true
            }
        ];
        yield return
        [
            new DueRecurrenceOptions
            {
                Recurrence = DueRecurrenceType.DayOfMonth,
                FirstDueDate = new DateTimeOffset(new DateTime(2099, 1, 8, 12, 0, 0, DateTimeKind.Utc))
            }
        ];
        yield return
        [
            new DueRecurrenceOptions
            {
                Recurrence = DueRecurrenceType.XthDayOfWeekOfTheMonth,
                FirstDueDate = new DateTimeOffset(new DateTime(2099, 1, 9, 12, 0, 0, DateTimeKind.Utc)),
                TimezoneId = "Europe/Copenhagen",
                XthPosition = DueRecurrenceXthPosition.Second
            }
        ];
    }

    [Fact]
    public async Task AddRemoveLabels()
    {
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Test: Add/Remove Labels
        Card? cardWithLabels = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Label Card"), cancellationToken: TestCancellationToken);
        Assert.Empty(cardWithLabels.LabelIds);

        Card? cardWithLabelsAdded = await TrelloClient.AddLabelsToCardAsync(cardWithLabels.Id, TestCancellationToken, allLabelsOnBoard.Select(x => x.Id).ToArray());
        Assert.Equal(allLabelsOnBoard.Count, cardWithLabelsAdded.LabelIds.Count);

        await TrelloClient.AddLabelsToCardAsync(cardWithLabels.Id, TestCancellationToken, allLabelsOnBoard.Select(x => x.Id).ToArray()); //Call once more to test it can handle added something already there

        Card? cardWithSingleLabelsRemoved = await TrelloClient.RemoveLabelsFromCardAsync(cardWithLabels.Id, TestCancellationToken, allLabelsOnBoard.First().Id);
        Assert.Equal(allLabelsOnBoard.Count - 1, cardWithSingleLabelsRemoved.LabelIds.Count);

        await TrelloClient.RemoveLabelsFromCardAsync(cardWithLabels.Id, TestCancellationToken, allLabelsOnBoard.First().Id); //Call once more to test it can handle removing something already not there

        Card? cardWithAllLabelsRemoved = await TrelloClient.RemoveAllLabelsFromCardAsync(cardWithLabels.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(cardWithAllLabelsRemoved.LabelIds);

        await TrelloClient.RemoveAllLabelsFromCardAsync(cardWithLabels.Id, cancellationToken: TestCancellationToken); //Call once more to test it can handle removing from already empty
    }

    [Fact]
    public async Task AddRemoveMembers()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<string> memberIds = new List<string> { member.Id };
        //Test: Add/Remove Members
        Card? cardWithMembers = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Member Card"), cancellationToken: TestCancellationToken);
        Assert.Empty(cardWithMembers.MemberIds);

        Card? cardWithMemberAdded = await TrelloClient.AddMembersToCardAsync(cardWithMembers.Id, TestCancellationToken, memberIds.ToArray());
        Assert.Single(cardWithMemberAdded.MemberIds);

        await TrelloClient.AddMembersToCardAsync(cardWithMembers.Id, TestCancellationToken, memberIds.ToArray()); //Call once more to test it can handle added something already there

        Card? cardWithSingleMemberRemoved = await TrelloClient.RemoveMembersFromCardAsync(cardWithMembers.Id, TestCancellationToken, memberIds.First());
        Assert.Empty(cardWithSingleMemberRemoved.MemberIds);

        await TrelloClient.RemoveMembersFromCardAsync(cardWithMembers.Id, TestCancellationToken, memberIds.First()); //Call once more to test if remove something not there works

        await TrelloClient.AddMembersToCardAsync(cardWithMembers.Id, TestCancellationToken, memberIds.ToArray()); //Re-add as test-board only have a single member
        Card? cardWithAllMemberRemoved = await TrelloClient.RemoveAllMembersFromCardAsync(cardWithMembers.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(cardWithAllMemberRemoved.MemberIds);
        await TrelloClient.RemoveAllMembersFromCardAsync(cardWithMembers.Id, cancellationToken: TestCancellationToken); //call once more to test it can handle already empty member list
    }

    [Fact]
    public async Task CustomDelete()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Custom Delete
        Card? customDeleteCard = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Custom Delete Card"), cancellationToken: TestCancellationToken);

        await TrelloClient.DeleteAsync($"cards/{customDeleteCard.Id}", cancellationToken: TestCancellationToken);
    }

    [Fact]
    public async Task Stickers()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Test Sticker CRUD
        Card? cardForStickerTests = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Sticker Tests"), cancellationToken: TestCancellationToken);
        List<Sticker>? stickersPresentJustAfterAdd = await TrelloClient.GetStickersOnCardAsync(cardForStickerTests.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(stickersPresentJustAfterAdd);
        Sticker? addedSticker = await TrelloClient.AddStickerToCardAsync(cardForStickerTests.Id, new Sticker(StickerDefaultImageId.Clock, 20, 10, 1, 45), cancellationToken: TestCancellationToken);
        Assert.NotEmpty(addedSticker.Id);
        Assert.NotEmpty(addedSticker.ImageUrl);
        Assert.Equal(StickerDefaultImageId.Clock, addedSticker.ImageIdAsDefaultEnum);
        Assert.Equal("clock", addedSticker.ImageId);
        Assert.Equal(10, addedSticker.Left);
        Assert.Equal(20, addedSticker.Top);
        Assert.Equal(1, addedSticker.ZIndex);
        Assert.Equal(45, addedSticker.Rotation);
        List<Sticker>? stickersPresentAfterOneAdded = await TrelloClient.GetStickersOnCardAsync(cardForStickerTests.Id, cancellationToken: TestCancellationToken);
        Assert.Single(stickersPresentAfterOneAdded);
        addedSticker.Left = 0;
        addedSticker.Top = 1;
        addedSticker.ZIndex = 2;
        addedSticker.Rotation = 3;
        Sticker? updateSticker = await TrelloClient.UpdateStickerAsync(cardForStickerTests.Id, addedSticker, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(updateSticker.Id);
        Assert.NotEmpty(updateSticker.ImageUrl);
        Assert.Equal(StickerDefaultImageId.Clock, updateSticker.ImageIdAsDefaultEnum);
        Assert.Equal("clock", updateSticker.ImageId);
        Assert.Equal(0, updateSticker.Left);
        Assert.Equal(1, updateSticker.Top);
        Assert.Equal(2, updateSticker.ZIndex);
        Assert.Equal(3, updateSticker.Rotation);
        Sticker? getSticker = await TrelloClient.GetStickerAsync(cardForStickerTests.Id, updateSticker.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(updateSticker.Id, getSticker.Id);
        Assert.Equal(updateSticker.ImageUrl, getSticker.ImageUrl);
        Assert.Equal(StickerDefaultImageId.Clock, getSticker.ImageIdAsDefaultEnum);
        Assert.Equal("clock", getSticker.ImageId);
        Assert.Equal(0, getSticker.Left);
        Assert.Equal(1, getSticker.Top);
        Assert.Equal(2, getSticker.ZIndex);
        Assert.Equal(3, getSticker.Rotation);
        await TrelloClient.DeleteStickerAsync(cardForStickerTests.Id, getSticker.Id, cancellationToken: TestCancellationToken);
        List<Sticker>? stickersPresentAfterDelete = await TrelloClient.GetStickersOnCardAsync(cardForStickerTests.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(stickersPresentAfterDelete);
    }

    [Fact]
    public async Task CommentReactions()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Test Comments
        Card? cardForComments = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Comments Tests"), cancellationToken: TestCancellationToken);

        Comment commentInput = new Comment("Hello World");
        TrelloAction? comment = await TrelloClient.AddCommentAsync(cardForComments.Id, commentInput, cancellationToken: TestCancellationToken);

        CommentReaction reaction = await TrelloClient.AddCommentReactionAsync(comment.Id, new Reaction
        {
            Native = "👍"
        }, cancellationToken: TestCancellationToken);

        Assert.Equal("👍", reaction.Emoji.Native);
        Assert.NotNull(reaction.MemberId);
        Assert.NotNull(reaction.EmojiId);
        Assert.NotNull(reaction.Id);
        Assert.NotNull(reaction.CommentId);
        Assert.NotNull(reaction.Member);
        Assert.NotNull(reaction.Member.FullName);
        Assert.NotNull(reaction.Emoji.Name);
        Assert.NotNull(reaction.Emoji.ShortName);
        Assert.NotNull(reaction.Emoji.UnifiedId);

        List<CommentReaction>? reactions = await TrelloClient.GetCommentReactionsAsync(comment.Id, cancellationToken: TestCancellationToken);
        Assert.Single(reactions);
        Assert.Equal("👍", reactions[0].Emoji.Native);

        await TrelloClient.DeleteReactionFromCommentAsync(comment.Id, reaction.Id, cancellationToken: TestCancellationToken);
        reactions = await TrelloClient.GetCommentReactionsAsync(comment.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(reactions);
    }

    [Fact]
    public async Task Comments()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        //Test Comments
        Card? cardForComments = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Comments Tests"), cancellationToken: TestCancellationToken);
        // ReSharper disable once RedundantAssignment
#pragma warning disable IDE0059
        Comment commentInput = new Comment(); //For Test-coverage
#pragma warning restore IDE0059
        commentInput = new Comment("Hello World");
        TrelloAction? comment = await TrelloClient.AddCommentAsync(cardForComments.Id, commentInput, cancellationToken: TestCancellationToken);
        Assert.NotEmpty(comment.Id);
        Assert.Equal("Hello World", comment.Data.Text);

        List<TrelloAction> commentsOnCard = await TrelloClient.GetPagedCommentsOnCardAsync(cardForComments.Id, cancellationToken: TestCancellationToken);
        Assert.Single(commentsOnCard);
        Assert.Equal(comment.Id, commentsOnCard.First().Id);
        Assert.Equal("Hello World", commentsOnCard.First().Data.Text);

        //Test UpdateComments
        comment.Data.Text = "New Text";
        TrelloAction? updatedComment = await TrelloClient.UpdateCommentActionAsync(comment, cancellationToken: TestCancellationToken);
        Assert.Equal(comment.Id, updatedComment.Id);
        Assert.Equal("New Text", updatedComment.Data.Text);

        //Test Delete Comments
        await TrelloClient.DeleteCommentActionAsync(comment.Id, cancellationToken: TestCancellationToken);

        List<TrelloAction> commentsOnCardAfterDelete = await TrelloClient.GetPagedCommentsOnCardAsync(cardForComments.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(commentsOnCardAfterDelete);


        List<TrelloAction> allCommentsOnCardAfterDelete = await TrelloClient.GetAllCommentsOnCardAsync(cardForComments.Id, cancellationToken: TestCancellationToken);
        Assert.Empty(allCommentsOnCardAfterDelete);

        for (int i = 0; i < 51; i++)
        {
            await TrelloClient.AddCommentAsync(cardForComments.Id, new Comment("Comment " + i), cancellationToken: TestCancellationToken);
        }

        List<TrelloAction> moreThan50Comments = await TrelloClient.GetAllCommentsOnCardAsync(cardForComments.Id, cancellationToken: TestCancellationToken);
        Assert.Equal(51, moreThan50Comments.Count);
    }

    [Fact]
    public async Task Covers()
    {
        List? list = await TrelloClient.AddListAsync(new List("List for Card Tests", _board.Id), cancellationToken: TestCancellationToken);
        Card? card = await TrelloClient.AddCardAsync(new AddCardOptions(list.Id, "Cover Tests"), cancellationToken: TestCancellationToken);
        Assert.Null(card.Cover.Color);

        Card? updatedCard = await TrelloClient.UpdateCoverOnCardAsync(card.Id, new CardCover(CardCoverColor.Blue, CardCoverSize.Full), cancellationToken: TestCancellationToken);
        Assert.Equal(CardCoverColor.Blue, updatedCard.Cover.Color);
        Assert.Equal(CardCoverSize.Full, updatedCard.Cover.Size);

        Card? updated2Card = await TrelloClient.RemoveCoverFromCardAsync(card.Id, cancellationToken: TestCancellationToken);
        Assert.Null(updated2Card.Cover.Color);

        Card? addCoverOnCardAsync = await TrelloClient.AddCoverToCardAsync(updated2Card.Id, new CardCover(CardCoverColor.Lime, CardCoverSize.Normal), cancellationToken: TestCancellationToken);
        Assert.Equal(CardCoverColor.Lime, addCoverOnCardAsync.Cover.Color);
        Assert.Equal(CardCoverSize.Normal, addCoverOnCardAsync.Cover.Size);

        Card? updateCoverOnCardAsync = await TrelloClient.UpdateCoverOnCardAsync(addCoverOnCardAsync.Id, new CardCover(CardCoverColor.Purple, CardCoverSize.Normal), cancellationToken: TestCancellationToken);
        Assert.Equal(CardCoverColor.Purple, updateCoverOnCardAsync.Cover.Color);
        Assert.Equal(CardCoverSize.Normal, updateCoverOnCardAsync.Cover.Size);

        await Assert.ThrowsAsync<TrelloApiException>(async () => await TrelloClient.UpdateCoverOnCardAsync("", null, cancellationToken: TestCancellationToken));

        Card? removeCoverFromCardAsync = await TrelloClient.RemoveCoverFromCardAsync(updateCoverOnCardAsync.Id, cancellationToken: TestCancellationToken);
        Assert.Null(removeCoverFromCardAsync.Cover.Color);
    }

    [Fact]
    public async Task MoveCards()
    {
        List list1 = await AddDummyList(_board.Id);
        List list2 = await AddDummyList(_board.Id);

        Card card1 = await AddDummyCardToList(list1);
        // ReSharper disable once UnusedVariable
        Card card2 = await AddDummyCardToList(list1);
        // ReSharper disable once UnusedVariable
        Card card3 = await AddDummyCardToList(list1);

        Card card4 = await AddDummyCardToList(list2);
        Card card5 = await AddDummyCardToList(list2);
        // ReSharper disable once UnusedVariable
        Card card6 = await AddDummyCardToList(list2);

        await TrelloClient.MoveCardToListAsync(card1.Id, list2.Id, new MoveCardToListOptions
        {
            NamedPositionOnNewList = NamedPosition.Top
        }, cancellationToken: TestCancellationToken);

        List<Card>? list1Cards = await TrelloClient.GetCardsInListAsync(list1.Id, cancellationToken: TestCancellationToken);
        List<Card>? list2Cards = await TrelloClient.GetCardsInListAsync(list2.Id, cancellationToken: TestCancellationToken);

        Assert.Equal(2, list1Cards.Count);
        Assert.Equal(4, list2Cards.Count);

        Assert.Equal(card1.Id, list2Cards.OrderBy(x => x.Position).First().Id);

        await TrelloClient.MoveCardToBottomOfCurrentListAsync(card1.Id, cancellationToken: TestCancellationToken);
        list2Cards = await TrelloClient.GetCardsInListAsync(list2.Id, cancellationToken: TestCancellationToken);

        Assert.Equal(card4.Id, list2Cards.OrderBy(x => x.Position).First().Id);

        await TrelloClient.MoveCardToTopOfCurrentListAsync(card5.Id, cancellationToken: TestCancellationToken);

        list2Cards = await TrelloClient.GetCardsInListAsync(list2.Id, cancellationToken: TestCancellationToken);

        Assert.Equal(card5.Id, list2Cards.OrderBy(x => x.Position).First().Id);
    }

    [Fact]
    public async Task GetCardsOnBoard()
    {
        List list1 = await AddDummyList(_board.Id);

        await AddDummyCardToList(list1);
        await AddDummyCardToList(list1);
        await AddDummyCardToList(list1);

        List<Card>? cards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, new GetCardOptions
        {
            Filter = CardsFilter.All,
            BoardFields = new BoardFields(BoardFieldsType.Name),
            ActionsTypes = new ActionTypesToInclude(WebhookActionTypes.CreateCard),
            CardFields = new CardFields(CardFieldsType.Name),
            Limit = 999,
            OrderBy = CardsOrderBy.CreateDateAsc,
            IncludeBoard = true,
            FilterConditions = [CardsFilterCondition.Name(CardsConditionString.NoneOfThese, Guid.NewGuid().ToString())],
            AdditionalParameters = [new QueryParameter("x", "z")],
            AttachmentFields = AttachmentFields.All,
            Before = "",
            ChecklistFields = ChecklistFields.All,
            IncludeAttachments = GetCardOptionsIncludeAttachments.True,
            IncludeChecklists = true,
            IncludeCustomFieldItems = true,
            IncludeList = true,
            IncludeMemberVotes = true,
            IncludeMembers = true,
            IncludePluginData = true,
            IncludeStickers = true,
            MemberFields = MemberFields.All,
            MembersVotedFields = MemberFields.All,
            Since = "",
            StickerFields = StickerFields.All
        }, cancellationToken: TestCancellationToken);

        foreach (Card card in cards)
        {
            Assert.NotNull(card.List);
            Assert.NotNull(card.Board);
            Assert.NotNull(card.Checklists);
            Assert.NotNull(card.Stickers);
            Assert.NotNull(card.Actions);
            Assert.NotNull(card.Attachments);
        }
    }

    [Fact]
    public async Task GetCardsOnBoardWithPageSize()
    {
        await using TemporaryBoardContext boardContext = await CreateTemporaryBoardAsync(nameof(GetCardsOnBoardWithPageSize));
        List list = await AddDummyList(boardContext.Board.Id);
        Card card1 = await AddDummyCardToList(list, "Paged 1");
        Card card2 = await AddDummyCardToList(list, "Paged 2");
        Card card3 = await AddDummyCardToList(list, "Paged 3");
        Card card4 = await AddDummyCardToList(list, "Paged 4");
        GetCardOptions options = new GetCardOptions
        {
            PageSize = 2,
            Limit = 3,
            FilterConditions = [CardsFilterCondition.ListId(CardsConditionIds.Equal, list.Id)],
            OrderBy = CardsOrderBy.CreateDateAsc
        };

        List<Card>? cards = await TrelloClient.GetCardsOnBoardAsync(boardContext.Board.Id, options, cancellationToken: TestCancellationToken);

        Assert.Equal(3, cards.Count);
        Assert.All(cards, card => Assert.Contains(card.Id, new[] { card1.Id, card2.Id, card3.Id, card4.Id }));
        Assert.Equal(3, options.Limit);
        Assert.Null(options.Before);
        Assert.Null(options.Since);
    }

    [Fact]
    public async Task GetCardsForMember()
    {
        List list1 = await AddDummyList(_board.Id);

        await AddDummyCardToList(list1);
        await AddDummyCardToList(list1);
        await AddDummyCardToList(list1);

        Member tokenMemberAsync = await TrelloClient.GetTokenMemberAsync(cancellationToken: TestCancellationToken);
        List<Card>? cards = await TrelloClient.GetCardsForMemberAsync(tokenMemberAsync.Id, new GetCardOptions
        {
            Filter = CardsFilter.All,
            BoardFields = new BoardFields(BoardFieldsType.Name),
            ActionsTypes = new ActionTypesToInclude(WebhookActionTypes.CreateCard),
            CardFields = new CardFields(CardFieldsType.Name),
            Limit = 999,
            OrderBy = CardsOrderBy.CreateDateAsc,
            IncludeBoard = true,
            FilterConditions = [CardsFilterCondition.Name(CardsConditionString.NoneOfThese, Guid.NewGuid().ToString())],
            AdditionalParameters = [new QueryParameter("x", "z")],
            AttachmentFields = AttachmentFields.All,
            Before = "",
            ChecklistFields = ChecklistFields.All,
            IncludeAttachments = GetCardOptionsIncludeAttachments.True,
            IncludeChecklists = true,
            IncludeCustomFieldItems = true,
            IncludeList = true,
            IncludeMemberVotes = true,
            IncludeMembers = true,
            IncludePluginData = true,
            IncludeStickers = true,
            MemberFields = MemberFields.All,
            MembersVotedFields = MemberFields.All,
            Since = "",
            StickerFields = StickerFields.All
        }, cancellationToken: TestCancellationToken);

        foreach (Card card in cards)
        {
            Assert.NotNull(card.List);
            Assert.NotNull(card.Board);
            Assert.NotNull(card.Checklists);
            Assert.NotNull(card.Stickers);
            Assert.NotNull(card.Actions);
            Assert.NotNull(card.Attachments);
        }
    }

    [Theory]
    [InlineData(false, false, MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndCreateMissing, MoveCardToBoardOptionsMemberOptions.KeepMembersAlsoOnNewBoardAndRemoveRest)]
    [InlineData(true, true, MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndCreateMissing, MoveCardToBoardOptionsMemberOptions.KeepMembersAlsoOnNewBoardAndRemoveRest)]
    [InlineData(true, true, MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndRemoveMissing, MoveCardToBoardOptionsMemberOptions.RemoveAllMembersOnCard)]
    [InlineData(true, true, MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndCreateMissing, MoveCardToBoardOptionsMemberOptions.KeepMembersAlsoOnNewBoardAndRemoveRest)]
    [InlineData(true, true, MoveCardToBoardOptionsLabelOptions.RemoveAllLabelsOnCard, MoveCardToBoardOptionsMemberOptions.RemoveAllMembersOnCard)]
    public async Task MoveCardToBoard(bool removeDueDate, bool removeStartDate, MoveCardToBoardOptionsLabelOptions labelOptions, MoveCardToBoardOptionsMemberOptions memberOptions)
    {
        List<Card>? existingCards = await TrelloClient.GetCardsOnBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        foreach (Card existingCard in existingCards)
        {
            await TrelloClient.DeleteCardAsync(existingCard.Id, cancellationToken: TestCancellationToken);
        }

        Board board = new Board("UnitTestBoard-" + Guid.NewGuid().ToString())
        {
            OrganizationId = _board.OrganizationId
        };
        Board secondBoard = await TrelloClient.AddBoardAsync(board, cancellationToken: TestCancellationToken);

        List list1 = await AddDummyList(_board.Id);
        List list2 = await AddDummyList(secondBoard.Id);

        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);
        foreach (Label label in allLabelsOnBoard)
        {
            label.Name = Guid.NewGuid().ToString();
            await TrelloClient.UpdateLabelAsync(label, cancellationToken: TestCancellationToken);
        }

        Card card1 = await AddDummyCardToList(list1, start: DateTimeOffset.UtcNow, due: DateTimeOffset.UtcNow.AddDays(1));
        await TrelloClient.AddMembersToCardAsync(card1.Id, TestCancellationToken, member.Id);
        await TrelloClient.AddLabelsToCardAsync(card1.Id, TestCancellationToken, allLabelsOnBoard.Select(x => x.Id).ToArray());

        await TrelloClient.MoveCardToBoardAsync(card1.Id, secondBoard.Id, new MoveCardToBoardOptions
        {
            NewListId = list2.Id,
            NamedPositionOnNewList = NamedPosition.Bottom,
            PositionOnNewList = 12,
            LabelOptions = labelOptions,
            MemberOptions = memberOptions,
            RemoveDueDate = removeDueDate,
            RemoveStartDate = removeStartDate
        }, cancellationToken: TestCancellationToken);

        List<Card>? cardsOnBoardAsync = await TrelloClient.GetCardsOnBoardAsync(secondBoard.Id, cancellationToken: TestCancellationToken);
        Assert.Single(cardsOnBoardAsync);
        Card card = cardsOnBoardAsync[0];
        Assert.Equal(card1.Id, card.Id);
        Assert.True(removeDueDate ? card.Due == null : card.Due != null);
        Assert.True(removeStartDate ? card.Start == null : card.Start != null);
        switch (labelOptions)
        {
            case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndColorAndCreateMissing:
                Assert.Equal(allLabelsOnBoard.Count, card.LabelIds.Count);
                break;
            case MoveCardToBoardOptionsLabelOptions.MigrateToLabelsOfSameNameAndRemoveMissing:
                Assert.Equal(allLabelsOnBoard.Count, card.LabelIds.Count);
                break;
            case MoveCardToBoardOptionsLabelOptions.RemoveAllLabelsOnCard:
                Assert.Empty(card.LabelIds);
                break;
        }
    }

    [Fact]
    public async Task MirrorCard()
    {
        Board board = new Board("UnitTestBoard-" + Guid.NewGuid())
        {
            OrganizationId = _board.OrganizationId
        };
        Board secondBoard = await TrelloClient.AddBoardAsync(board, cancellationToken: TestCancellationToken);

        List sourceList = await AddDummyList(_board.Id);
        List targetList = await AddDummyList(secondBoard.Id);

        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);

        Card sourceCard = await AddDummyCardToList(sourceList,
            start: DateTimeOffset.UtcNow,
            due: DateTimeOffset.UtcNow.AddDays(1),
            description: "Test Description");

        await TrelloClient.AddMembersToCardAsync(sourceCard.Id, TestCancellationToken, member.Id);
        await TrelloClient.AddLabelsToCardAsync(sourceCard.Id, TestCancellationToken, allLabelsOnBoard.Select(x => x.Id).ToArray());

        Card? mirroredCard = await TrelloClient.MirrorCardAsync(new MirrorCardOptions
        {
            SourceCardId = sourceCard.Id,
            TargetListId = targetList.Id,
            NamedPosition = NamedPosition.Bottom
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(sourceCard.Id, mirroredCard.Id);
        Assert.Equal(sourceCard.ShortUrl, mirroredCard.Name);
        Assert.Equal(targetList.Id, mirroredCard.ListId);
        Assert.Equal(secondBoard.Id, mirroredCard.BoardId);
    }

    [Fact]
    public async Task MirrorCardWithExplicitPosition()
    {
        Board board = new Board("UnitTestBoard-" + Guid.NewGuid())
        {
            OrganizationId = _board.OrganizationId
        };
        Board secondBoard = await TrelloClient.AddBoardAsync(board, cancellationToken: TestCancellationToken);

        List sourceList = await AddDummyList(_board.Id);
        List targetList = await AddDummyList(secondBoard.Id);
        Card sourceCard = await AddDummyCardToList(sourceList, description: "Test Description");

        Card? mirroredCard = await TrelloClient.MirrorCardAsync(new MirrorCardOptions
        {
            SourceCardId = sourceCard.Id,
            TargetListId = targetList.Id,
            Position = 1234
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(sourceCard.Id, mirroredCard.Id);
        Assert.Equal(targetList.Id, mirroredCard.ListId);
    }

    [Fact]
    public async Task AddCardFromTemplate()
    {
        List sourceList = await AddDummyList(_board.Id);
        List targetList = await AddDummyList(_board.Id);

        Member? member = (await TrelloClient.GetMembersOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken)).First();
        List<Label>? allLabelsOnBoard = await TrelloClient.GetLabelsOfBoardAsync(_board.Id, cancellationToken: TestCancellationToken);

        Card? templateCard = await AddDummyCardToList(sourceList,
            start: DateTimeOffset.UtcNow,
            due: DateTimeOffset.UtcNow.AddDays(1),
            description: "Template Description");

        templateCard = await TrelloClient.AddMembersToCardAsync(templateCard.Id, TestCancellationToken, member.Id);
        templateCard = await TrelloClient.AddLabelsToCardAsync(templateCard.Id, TestCancellationToken, allLabelsOnBoard.Select(x => x.Id).ToArray());

        Card? newCard = await TrelloClient.AddCardFromTemplateAsync(new AddCardFromTemplateOptions
        {
            SourceTemplateCardId = templateCard.Id,
            TargetListId = targetList.Id,
            Name = "New Card From Template",
            NamedPosition = NamedPosition.Bottom,
            Keep = AddCardFromTemplateOptionsToKeep.All
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(templateCard.Id, newCard.Id);
        Assert.Equal("New Card From Template", newCard.Name);
        Assert.Equal(targetList.Id, newCard.ListId);
        Assert.Equal(_board.Id, newCard.BoardId);
        Assert.Equal(templateCard.Description, newCard.Description);
        Assert.Equal(templateCard.Start, newCard.Start);
        Assert.Equal(templateCard.Due, newCard.Due);
        Assert.Equal(templateCard.MemberIds.Count, newCard.MemberIds.Count);
        Assert.Equal(templateCard.LabelIds.Count, newCard.LabelIds.Count);
    }

    [Fact]
    public async Task AddCardFromTemplateWithExplicitPositionAndPartialKeep()
    {
        List sourceList = await AddDummyList(_board.Id);
        List targetList = await AddDummyList(_board.Id);
        Card? templateCard = await AddDummyCardToList(sourceList,
            start: DateTimeOffset.UtcNow,
            due: DateTimeOffset.UtcNow.AddDays(1),
            description: "Template Description");

        Card? newCard = await TrelloClient.AddCardFromTemplateAsync(new AddCardFromTemplateOptions
        {
            SourceTemplateCardId = templateCard.Id,
            TargetListId = targetList.Id,
            Position = 1234,
            Keep = AddCardFromTemplateOptionsToKeep.Due | AddCardFromTemplateOptionsToKeep.Start
        }, cancellationToken: TestCancellationToken);

        Assert.NotEqual(templateCard.Id, newCard.Id);
        Assert.Equal(templateCard.Name, newCard.Name);
        Assert.Equal(targetList.Id, newCard.ListId);
        Assert.Equal(templateCard.Start, newCard.Start);
        Assert.Equal(templateCard.Due, newCard.Due);
        Assert.Equal(templateCard.Description, newCard.Description);
    }
}
