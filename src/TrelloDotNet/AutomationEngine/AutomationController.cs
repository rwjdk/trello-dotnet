using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.AutomationEngine.Interface;
using TrelloDotNet.AutomationEngine.Model;
using TrelloDotNet.Control.Webhook;
using TrelloDotNet.Model.Webhook;

namespace TrelloDotNet.AutomationEngine
{
    /// <summary>
    /// The Automation Controller that can be used to automate receiving of Webhooks events against a set of configured automations
    /// </summary>
    public class AutomationController
    {
        private readonly Automation[] _automations;
        private readonly WebhookDataReceiver _receiver;
        private readonly TrelloClient _trelloClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration of the Automation Engine</param>
        public AutomationController(Configuration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration), "You did not provide a configuration");
            }

            _trelloClient = configuration.TrelloClient;
            _automations = configuration.Automations;
            _receiver = new WebhookDataReceiver(_trelloClient);
        }

        /// <summary>
        /// Process raw JSON from a Trello Webhook against the provided set of Automations in the configuration
        /// </summary>
        /// <param name="request">The processing request (with the JSON)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        public async Task<ProcessingResult> ProcessJsonFromWebhookAsync(ProcessingRequest request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ProcessingResult result = new ProcessingResult();

            if ((!string.IsNullOrEmpty(request.Signature) || !string.IsNullOrEmpty(request.WebhookUrl))
                && !WebhookSignatureValidator.ValidateSignature(request.JsonFromWebhook, request.Signature, request.WebhookUrl, _trelloClient.Options.Secret))
            {
                result.AddToLog("Webhook Signature Validation failed, skipping automations");
                return result;
            }

            WebhookNotification data = _receiver.ConvertJsonToWebhookNotification(request.JsonFromWebhook);
            WebhookAction webhookAction = data.Action;

            foreach (Automation automation in _automations)
            {
                IAutomationTrigger triggerBeingChecked = null;
                try
                {
                    bool triggerMet = false;
                    cancellationToken.ThrowIfCancellationRequested();
                    foreach (IAutomationTrigger trigger in automation.Triggers)
                    {
                        triggerBeingChecked = trigger;
                        triggerMet = await trigger.IsTriggerMetAsync(webhookAction);
                        if (triggerMet)
                        {
                            break;
                        }
                    }

                    if (!triggerMet)
                    {
                        result.AddToLog($"Automation '{automation.Name}' was not processed as trigger was not met");
                        result.AutomationsSkipped++;
                        continue; //Wrong Trigger
                    }
                }
                catch (Exception e)
                {
                    if (triggerBeingChecked != null)
                    {
                        throw new AutomationException($"Error checking Trigger of type '{triggerBeingChecked.GetType()}' in automation '{automation.Name}'{AddErrorContext(data)}", e);
                    }

                    throw new AutomationException($"Error checking Trigger in automation '{automation.Name}'{AddErrorContext(data)}", e);
                }

                bool conditionsMet = true;
                if (automation.Conditions != null)
                {
                    foreach (IAutomationCondition x in automation.Conditions)
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            if (!await x.IsConditionMetAsync(webhookAction))
                            {
                                conditionsMet = false;
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new AutomationException($"Error checking Condition of type '{x.GetType()}' in automation '{automation.Name}'{AddErrorContext(data)}", e);
                        }
                    }
                }

                if (conditionsMet)
                {
                    result.AutomationsProcessed++;
                    result.AddToLog($"Automation '{automation.Name}' trigger and condition was met - Executing {automation.Actions.Count} Action");
                    foreach (IAutomationAction actionAction in automation.Actions.Where(x => x != null))
                    {
                        try
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            await actionAction.PerformActionAsync(webhookAction, result);
                        }
                        catch (StopProcessingFurtherActionException)
                        {
                            return result;
                        }
                        catch (Exception e)
                        {
                            throw new AutomationException($"Error performing Action of type '{actionAction.GetType()}' in automation '{automation.Name}'{AddErrorContext(data)}", e);
                        }
                    }
                }
                else
                {
                    result.AddToLog($"Automation '{automation.Name}' was not processed as not all conditions where met was not met");
                    result.AutomationsSkipped++;
                }
            }

            return result;
        }

        private string AddErrorContext(WebhookNotification webhookNotification)
        {
            try
            {
                return webhookNotification.Action.SummarizeEvent();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}