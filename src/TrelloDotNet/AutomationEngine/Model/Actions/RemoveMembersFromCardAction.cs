using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Actions
{
    /// <summary>
    /// Remove one or more Members from a card
    /// </summary>
    public class RemoveMembersFromCardAction : IAutomationAction
    {
        /// <summary>
        /// Member Id's to remove
        /// </summary>
        public string[] MemberIds { get; }

        /// <summary>
        /// Set this to 'True' if you supplied names of Member Full-names instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things.
        /// </summary>
        public bool TreatMemberNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="memberIds">Member Id's to remove</param>
        public RemoveMembersFromCardAction(params string[] memberIds)
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
                throw new AutomationException("Could not perform RemoveMembersFromCardAction as WebhookAction did not involve a Card");
            }

            string[] memberIdsToRemove = MemberIds;

            if (TreatMemberNameAsId)
            {
                List<Member> allMembers = await webhookAction.TrelloClient.GetMembersOfBoardAsync(webhookAction.Data.Board.Id);
                List<string> idsFromNames = new List<string>();
                foreach (string memberName in MemberIds) //Remember; here the 'Id's' are actually Names so we need to find the id's needed for removal
                {
                    Member member = allMembers.FirstOrDefault(x => x.FullName == memberName);
                    if (member != null)
                    {
                        idsFromNames.Add(member.Id);
                    }
                }

                memberIdsToRemove = idsFromNames.ToArray();
            }

            await webhookAction.TrelloClient.RemoveMembersFromCardAsync(webhookAction.Data.Card.Id, memberIdsToRemove);
            processingResult.ActionsExecuted++;
        }
    }
}