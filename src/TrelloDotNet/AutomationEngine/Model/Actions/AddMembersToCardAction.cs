using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// This Automation Action adds a set of Members to a card if they are not already present.
    /// </summary>
    public class AddMembersToCardAction : IAutomationAction
    {
        /// <summary>
        /// The Member-ids to add
        /// </summary>
        public string[] MemberIds { get; }

        /// <summary>
        /// Set this to 'True' if you supplied names of members instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and are less resilient to renaming of things.
        /// </summary>
        public bool TreatMemberNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberIds">The Label-ids to add</param>
        public AddMembersToCardAction(params string[] memberIds)
        {
            MemberIds = memberIds;
        }

        /// <summary>
        /// The method called when an automation should be performed
        /// </summary>
        /// <param name="webhookAction">The Webhook Action that led to the Execution. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <param name="processingResult">An object you can use to report back to the user if the action was performed and details about it</param>
        /// <returns>Void</returns>
        public async Task PerformActionAsync(WebhookAction webhookAction, ProcessingResult processingResult)
        {
            if (webhookAction.Data?.Card == null)
            {
                throw new AutomationException("Could not perform AddMembersToCardAction as WebhookAction did not involve a Card");
            }

            TrelloClient trelloClient = webhookAction.TrelloClient;

            string[] memberIdsToAdd = MemberIds;

            if (TreatMemberNameAsId)
            {
                List<Member> allMembers = await webhookAction.TrelloClient.GetMembersOfBoardAsync(webhookAction.Data.Board.Id);
                List<string> idsFromNames = new List<string>();
                foreach (string memberName in MemberIds) //Remember; here the 'Id's' are actually Names so we need to find the id's needed for add
                {
                    Member member = allMembers.FirstOrDefault(x => x.FullName == memberName);
                    if (member != null)
                    {
                        idsFromNames.Add(member.Id);
                    }
                }

                memberIdsToAdd = idsFromNames.ToArray();
            }

            Card card = await webhookAction.Data.Card.GetAsync();
            bool updateNeeded = false;
            foreach (string memberId in memberIdsToAdd)
            {
                if (!card.MemberIds.Contains(memberId))
                {
                    card.MemberIds.Add(memberId);
                    updateNeeded = true;
                }
            }

            if (updateNeeded)
            {
                await trelloClient.UpdateCardAsync(card.Id, new List<CardUpdate>()
                {
                    CardUpdate.Members(card.MemberIds)
                });
                processingResult.AddToLog($"Added members '{string.Join(",", MemberIds)}' to card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsExecuted++;
            }
            else
            {
                processingResult.AddToLog($"SKIPPED: Adding members as they are already on card '{webhookAction.Data.Card.Name}'");
                processingResult.ActionsSkipped++;
            }
        }
    }
}