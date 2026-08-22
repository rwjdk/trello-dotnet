using System;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Options;
using TrelloDotNet.Model.Options.GetCardOptions;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine.Model.Conditions
{
    /// <summary>
    /// Check if a Card has a specific Cover or not
    /// </summary>
    public class CardCoverCondition : IAutomationCondition
    {
        /// <summary>
        /// The constraint for the Cover
        /// </summary>
        public CardCoverConditionConstraint Constraint { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="constraint">The constraint for the Cover </param>
        public CardCoverCondition(CardCoverConditionConstraint constraint)
        {
            Constraint = constraint;
        }

        /// <summary>
        /// Method to check if the condition is met
        /// </summary>
        /// <param name="webhookAction">The Webhook Action that led to check of the condition. This object also have the TrelloClient and information about the event at your disposal</param>
        /// <returns>If condition is met or not</returns>
        public async Task<bool> IsConditionMetAsync(WebhookAction webhookAction)
        {
            if (webhookAction.Data?.Card == null)
            {
                throw new AutomationException("Could not perform CardCoverCondition.IsConditionMetAsync as WebhookAction did not involve a Card");
            }

            Card card = await webhookAction.Data.Card.GetAsync(new GetCardOptions
            {
                CardFields = new CardFields("cover")
            });
            switch (Constraint)
            {
                case CardCoverConditionConstraint.DoesNotHaveACover:
                    return card.Cover == null || (card.Cover.BackgroundImageId == null && (card.Cover.Color == null || card.Cover.Color == CardCoverColor.None));
                case CardCoverConditionConstraint.DoesNotHaveACoverOfTypeColor:
                    return card.Cover?.Color == null || card.Cover.Color == CardCoverColor.None;
                case CardCoverConditionConstraint.DoesNotHaveACoverOfTypeImage:
                    return card.Cover?.BackgroundImageId == null;
                case CardCoverConditionConstraint.HaveACover:
                    return card.Cover.BackgroundImageId != null || (card.Cover.Color != null && card.Cover.Color != CardCoverColor.None);
                case CardCoverConditionConstraint.HaveACoverOfTypeColor:
                    return card.Cover.Color != null && card.Cover.Color != CardCoverColor.None;
                case CardCoverConditionConstraint.HaveACoverOfTypeImage:
                    return card.Cover.BackgroundImageId != null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Constraint));
            }
        }
    }
}