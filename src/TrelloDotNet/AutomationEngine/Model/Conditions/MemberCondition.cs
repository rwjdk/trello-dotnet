using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Conditions
{
    /// <summary>
    /// Condition that checks Members on a Card is present/not present
    /// </summary>
    public class MemberCondition : IAutomationCondition
    {
        /// <summary>
        /// The constraints of the condition
        /// </summary>
        public MemberConditionConstraint Constraint { get; set; }

        /// <summary>
        /// The Ids of the Member or Members to check. Tip: These can be Member-names instead of Ids if you set 'TreatMemberNameAsId' to True
        /// </summary>
        public string[] MemberIds { get; set; }

        /// <summary>
        /// Set this to 'True' if you supplied the names of members instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things.
        /// </summary>
        public bool TreatMemberNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constraint">The constraints of the condition</param>
        /// <param name="memberIds">The Ids of the Member or Members to check. Tip: These can be Member-names instead of Ids if you set 'TreatMemberNameAsId' to True</param>
        public MemberCondition(MemberConditionConstraint constraint, params string[] memberIds)
        {
            Constraint = constraint;
            MemberIds = memberIds;
        }

        /// <summary>
        /// Method to check if the condition is met
        /// </summary>
        /// <param name="webhookAction">The Webhook Action that led to check of the condition. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <returns>If condition is met or not</returns>
        public async Task<bool> IsConditionMetAsync(WebhookAction webhookAction)
        {
            Card card = await webhookAction.Data.Card.GetAsync(new GetCardOptions
            {
                CardFields = new CardFields("idMembers")
            });
            string[] idsToCheck = MemberIds;
            if (TreatMemberNameAsId)
            {
                List<Member> allMembers = await webhookAction.TrelloClient.GetMembersOfBoardAsync(webhookAction.Data.Board.Id);
                idsToCheck = allMembers.Where(x => MemberIds.Contains(x.FullName)).Select(x => x.Id).ToArray();
            }

            switch (Constraint)
            {
                case MemberConditionConstraint.AnyOfThesePresent:
                    MemberIsNeededGuard();
                    return idsToCheck.Any(x => card.MemberIds.Contains(x));
                case MemberConditionConstraint.AllOfThesePresent:
                    MemberIsNeededGuard();
                    return idsToCheck.All(x => card.MemberIds.Contains(x));
                case MemberConditionConstraint.NoneOfTheseArePresent:
                    MemberIsNeededGuard();
                    return idsToCheck.All(x => !card.MemberIds.Contains(x));
                case MemberConditionConstraint.NonePresent:
                    return card.MemberIds.Count == 0;
                default:
                    throw new AutomationException($"Invalid MemberConditionConstraint '{Constraint}' sent to MemberCondition.IsConditionMetAsync");
            }
        }

        private void MemberIsNeededGuard()
        {
            if (!MemberIds.Any())
            {
                throw new AutomationException("MemberCondition: No MemberIds specified");
            }
        }
    }
}