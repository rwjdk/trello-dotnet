using System;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Triggers
{
    /// <summary>
    /// A Trigger that occurs when a Card has a Member removed
    /// </summary>
    public class MemberRemovedFromCardTrigger : IAutomationTrigger
    {
        /// <summary>
        /// Set this to 'True' if you supplied the names of Members instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things.
        /// </summary>
        public bool TreatMemberNameAsId { get; set; }

        /// <summary>
        /// Constraint of the trigger
        /// </summary>
        public MemberRemovedFromCardTriggerConstraint Constraint { get; }

        /// <summary>
        /// The Ids of the Member or Members to check. Tip: These can be Member-names instead of Ids if you set 'TreatMemberNameAsId' to True
        /// </summary>
        public string[] MemberIds { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constraint">Constraint of the trigger</param>
        /// <param name="memberIds">The Ids of the Member or Members to check. Tip: These can be Member-names instead of Ids if you set 'TreatMemberNameAsId' to True</param>
        public MemberRemovedFromCardTrigger(MemberRemovedFromCardTriggerConstraint constraint, params string[] memberIds)
        {
            Constraint = constraint;
            MemberIds = memberIds;
        }

        /// <summary>
        /// If the Trigger is met
        /// </summary>
        /// <remarks>
        /// While this is built to support async execution, It is best practice to keep a Trigger as light as possible only checking values against the webhook Action and not make any API Calls. The reason for this is that Triggers are called quite often, so it is better to make simple triggers and supplement with Conditions
        /// </remarks>
        /// <param name="webhookAction">The Webhook Action that led to the check of the trigger. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <returns>If trigger is met or not</returns>
        public async Task<bool> IsTriggerMetAsync(WebhookAction webhookAction)
        {
            await Task.CompletedTask;
            bool correctType = webhookAction.Type == WebhookActionTypes.RemoveMemberFromCard;
            string partToMatch = TreatMemberNameAsId ? webhookAction.Data?.Member?.Name : webhookAction.Data?.Member?.Id;
            switch (Constraint)
            {
                case MemberRemovedFromCardTriggerConstraint.AnyOfTheseMembersAreRemoved:
                    return correctType && MemberIds.Contains(partToMatch);
                case MemberRemovedFromCardTriggerConstraint.AnyButTheseMembersAreRemoved:
                    return correctType && !MemberIds.Contains(partToMatch);
                case MemberRemovedFromCardTriggerConstraint.AnyMember:
                    return correctType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}