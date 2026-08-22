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
    /// A Condition that checks labels on a Card is present/not present
    /// </summary>
    public class LabelCondition : IAutomationCondition
    {
        /// <summary>
        /// The constraints of the condition
        /// </summary>
        public LabelConditionConstraint Constraint { get; set; }

        /// <summary>
        /// The Ids of the label or Labels to check. Tip: These can be Label-names instead of Ids if you set 'TreatLabelNameAsId' to True
        /// </summary>
        public string[] LabelIds { get; set; }

        /// <summary>
        /// Set this to 'True' if you supplied the names of labels instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to the renaming of things.
        /// </summary>
        public bool TreatLabelNameAsId { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constraint">The constraints of the condition</param>
        /// <param name="labelIds">The Ids of the label or Labels to check. Tip: These can be Label-names instead of Ids if you set 'TreatLabelNameAsId' to True</param>
        public LabelCondition(LabelConditionConstraint constraint, params string[] labelIds)
        {
            Constraint = constraint;
            LabelIds = labelIds;
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
                CardFields = new CardFields("idLabels")
            });
            string[] labelIdsToCheck = LabelIds;
            if (TreatLabelNameAsId)
            {
                List<Label> allLabels = await webhookAction.TrelloClient.GetLabelsOfBoardAsync(webhookAction.Data.Board.Id);
                labelIdsToCheck = allLabels.Where(x => LabelIds.Contains(x.Name)).Select(x => x.Id).ToArray();
            }

            switch (Constraint)
            {
                case LabelConditionConstraint.AnyOfThesePresent:
                    LabelIdIsNeededGuard();
                    return labelIdsToCheck.Any(labelIdToCheck => card.LabelIds.Contains(labelIdToCheck));
                case LabelConditionConstraint.AllOfThesePresent:
                    LabelIdIsNeededGuard();
                    return labelIdsToCheck.All(labelIdToCheck => card.LabelIds.Contains(labelIdToCheck));
                case LabelConditionConstraint.NoneOfTheseArePresent:
                    LabelIdIsNeededGuard();
                    return labelIdsToCheck.All(labelIdToCheck => !card.LabelIds.Contains(labelIdToCheck));
                case LabelConditionConstraint.NonePresent:
                    return card.LabelIds.Count == 0;
                default:
                    throw new AutomationException($"Invalid LabelConditionConstraint '{Constraint}' sent to LabelCondition.IsConditionMetAsync");
            }
        }

        private void LabelIdIsNeededGuard()
        {
            if (!LabelIds.Any())
            {
                throw new AutomationException("LabelCondition: No LabelIds specified");
            }
        }
    }
}