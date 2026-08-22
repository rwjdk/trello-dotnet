using System;
using System.Linq;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Triggers
{
    /// <summary>
    /// A Trigger that occurs when a Card gets a new label Added
    /// </summary>
    public class LabelAddedToCardTrigger : IAutomationTrigger
    {
        /// <summary>
        /// Set this to 'True' if you supplied the names of labels instead of the Ids. While this is more convenient, it will, in certain cases, be slightly slower and less resilient to renaming things.
        /// </summary>
        public bool TreatLabelNameAsId { get; set; }

        /// <summary>
        /// The Constraint of the Trigger
        /// </summary>
        public LabelAddedToCardTriggerConstraint Constraint { get; }

        /// <summary>
        /// The Ids of the label or Labels to check. Tip: These can be Label-names instead of Ids if you set 'TreatLabelNameAsId' to True
        /// </summary>
        public string[] LabelIds { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="constraint">The Constraint of the Trigger</param>
        /// <param name="labelIds">The Ids of the label or Labels to check. Tip: These can be Label-names instead of Ids if you set 'TreatLabelNameAsId' to True</param>
        public LabelAddedToCardTrigger(LabelAddedToCardTriggerConstraint constraint, params string[] labelIds)
        {
            Constraint = constraint;
            LabelIds = labelIds;
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
            bool correctType = webhookAction.Type == WebhookActionTypes.AddLabelToCard;
            string partToMatch = TreatLabelNameAsId ? webhookAction.Data?.Label?.Name : webhookAction.Data?.Label?.Id;
            switch (Constraint)
            {
                case LabelAddedToCardTriggerConstraint.AnyOfTheseLabelsAreAdded:
                    return correctType && LabelIds.Contains(partToMatch);
                case LabelAddedToCardTriggerConstraint.AnyButTheseLabelsAreAreAdded:
                    return correctType && !LabelIds.Contains(partToMatch);
                case LabelAddedToCardTriggerConstraint.AnyLabel:
                    return correctType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}