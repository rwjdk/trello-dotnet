using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Extensions;
using TrelloDotNet.Model;

namespace TrelloDotNet
{
    /// <summary>
    /// The Client to communicate with the Trello API
    /// </summary>
    public partial class TrelloClient
    {
        /// <summary>
        /// Options for the client
        /// </summary>
        public TrelloClientOptions Options { get; }

        private readonly ApiRequestController _apiRequestController;
        private readonly QueryParametersBuilder _queryParametersBuilder;
        private readonly HttpClient _staticHttpClient = new HttpClient();

        /// <summary>
        /// Initializes a new instance of the TrelloClient for communicating with the Trello API using the provided API Key and Token.
        /// </summary>
        /// <param name="apiKey">The Trello API Key you get on https://trello.com/power-ups/admin/</param>
        /// <param name="token">Your Authorization Token you get on https://trello.com/power-ups/admin/</param>
        /// <param name="options">Optional client options; if null, default options will be used</param>
        /// <param name="httpClient">Optional HTTP Client to use for requests; if not provided, an internal static HttpClient will be used</param>
        public TrelloClient(string apiKey, string token, TrelloClientOptions options = null, HttpClient httpClient = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("You need to specify an API Key. Get it on page: https://trello.com/power-ups/admin/");
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("You need to specify a Token. Generate it on page: https://trello.com/power-ups/admin/");
            }

            if (httpClient != null)
            {
                _staticHttpClient = httpClient;
            }

            Options = options ?? new TrelloClientOptions();
            _apiRequestController = new ApiRequestController(_staticHttpClient, apiKey, token, this);
            _queryParametersBuilder = new QueryParametersBuilder();
        }

        /// <summary>
        /// Retrieves information about the token currently used by this TrelloClient instance.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Information about the token</returns>
        public async Task<TokenInformation> GetTokenInformationAsync(CancellationToken cancellationToken = default)
        {
            return await _apiRequestController.Get<TokenInformation>($"{UrlPaths.Tokens}/{_apiRequestController.Token}", cancellationToken);
        }

        /// <summary>
        /// Retrieves the inbox information (IDs) for the owner of the token used by this TrelloClient.
        /// </summary>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The inbox information for the token owner</returns>
        [Obsolete("In June 2026 Trello Stopped working with the Inbox over API. It is unknown if this is a bug or intentional choice, but added this warning for people to be aware")]
        public async Task<TokenMemberInbox> GetTokenMemberInboxAsync(CancellationToken cancellationToken = default)
        {
            TokenMemberInformationInbox inbox = (await _apiRequestController.Get<TokenMemberInformationInbox>($"{UrlPaths.Members}/me?fields=inbox", cancellationToken));
            if (inbox.Inbox != null)
            {
                List<Label> labels = await GetLabelsOfBoardAsync(inbox.Inbox.BoardId, cancellationToken);
                inbox.Inbox.Labels = labels;
                return inbox.Inbox;
            }

            return null;
        }

        private static List<Card> OrderCards(List<Card> cards, CardsOrderBy? orderBy)
        {
            switch (orderBy)
            {
                case CardsOrderBy.CreateDateAsc:
                    return cards.OrderBy(x => x.Created).ToList();
                case CardsOrderBy.CreateDateDesc:
                    return cards.OrderByDescending(x => x.Created).ToList();
                case CardsOrderBy.StartDateAsc:
                    return cards.OrderBy(x => x.Start).ToList();
                case CardsOrderBy.StartDateDesc:
                    return cards.OrderByDescending(x => x.Start).ToList();
                case CardsOrderBy.DueDateAsc:
                    return cards.OrderBy(x => x.Due).ToList();
                case CardsOrderBy.DueDateDesc:
                    return cards.OrderByDescending(x => x.Due).ToList();
                case CardsOrderBy.NameAsc:
                    return cards.OrderBy(x => x.Name).ToList();
                case CardsOrderBy.NameDesc:
                    return cards.OrderByDescending(x => x.Name).ToList();
            }

            return cards;
        }

        private List<Card> FilterCards(List<Card> cards, List<CardsFilterCondition> filterConditions)
        {
            if (filterConditions == null || filterConditions.Count == 0)
            {
                return cards;
            }

            return cards.Filter(filterConditions.ToArray());
        }
    }
}