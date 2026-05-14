using TrelloDotNet.Control;

namespace TrelloDotNet.Tests.UnitTests;

public class GetUrlBuilderTests
{
    [Theory]
    [InlineData("board1", "boards/board1/actions")]
    [InlineData("board with spaces", "boards/board with spaces/actions")]
    public void GetActionsOnBoardBuildsExpectedPath(string boardId, string expected)
    {
        Assert.Equal(expected, GetUrlBuilder.GetActionsOnBoard(boardId));
    }

    [Theory]
    [InlineData("card1", "cards/card1/actions")]
    [InlineData("card1", "cards/card1/attachments", "attachments")]
    public void CardRoutesBuildExpectedPaths(string cardId, string expected, string route = "actions")
    {
        string actual = route switch
        {
            "attachments" => GetUrlBuilder.GetAttachmentsOnCard(cardId),
            _ => GetUrlBuilder.GetActionsOnCard(cardId)
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetAttachmentOnCardBuildsNestedPath()
    {
        string value = GetUrlBuilder.GetAttachmentOnCard("card1", "attachment1");

        Assert.Equal("cards/card1/attachments/attachment1", value);
    }

    [Theory]
    [InlineData("list1", "lists/list1/actions")]
    [InlineData("member1", "members/member1/actions")]
    [InlineData("org1", "organizations/org1/actions")]
    public void ActionCollectionRoutesBuildExpectedPaths(string id, string expected)
    {
        string actual = expected.StartsWith("lists")
            ? GetUrlBuilder.GetActionsForList(id)
            : expected.StartsWith("members")
                ? GetUrlBuilder.GetActionsForMember(id)
                : GetUrlBuilder.GetActionsForOrganization(id);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ChecklistRoutesBuildExpectedPaths()
    {
        Assert.Equal("checklists/checklist1", GetUrlBuilder.GetChecklist("checklist1"));
        Assert.Equal("boards/board1/checklists", GetUrlBuilder.GetChecklistsOnBoard("board1"));
        Assert.Equal("cards/card1/checklists", GetUrlBuilder.GetChecklistsOnCard("card1"));
    }

    [Fact]
    public void MemberRoutesBuildExpectedPaths()
    {
        Assert.Equal("boards/board1/members/", GetUrlBuilder.GetMembersOfBoard("board1"));
        Assert.Equal("cards/card1/members/", GetUrlBuilder.GetMembersOfCard("card1"));
        Assert.Equal("cards/card1/membersVoted", GetUrlBuilder.GetMembersWhoVotedOnOfCard("card1"));
        Assert.Equal("members/member1", GetUrlBuilder.GetMember("member1"));
        Assert.Equal("organizations/org1/members/", GetUrlBuilder.GetMembersOfOrganization("org1"));
    }

    [Fact]
    public void OrganizationAndTokenRoutesBuildExpectedPaths()
    {
        Assert.Equal("tokens/token1/member", GetUrlBuilder.GetTokenMember("token1"));
        Assert.Equal("boards/board1/memberships", GetUrlBuilder.GetMembershipsOfBoard("board1"));
        Assert.Equal("organizations/org1", GetUrlBuilder.GetOrganization("org1"));
        Assert.Equal("members/member1/organizations", GetUrlBuilder.GetOrganizationsForMember("member1"));
    }

    [Fact]
    public void StickerWebhookAndPluginDataRoutesBuildExpectedPaths()
    {
        Assert.Equal("cards/card1/stickers", GetUrlBuilder.GetStickersOnCard("card1"));
        Assert.Equal("cards/card1/stickers/sticker1", GetUrlBuilder.GetSticker("card1", "sticker1"));
        Assert.Equal("tokens/token1/webhooks", GetUrlBuilder.GetWebhooksForToken("token1"));
        Assert.Equal("webhooks/webhook1", GetUrlBuilder.GetWebhook("webhook1"));
        Assert.Equal("cards/card1/pluginData", GetUrlBuilder.GetPluginDataOnCard("card1"));
        Assert.Equal("boards/board1/pluginData", GetUrlBuilder.GetPluginDataOfBoard("board1"));
    }
}
