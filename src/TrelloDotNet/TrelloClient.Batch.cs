using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Control;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Batch;
using TrelloDotNet.Model.Options.GetBoardOptions;
using TrelloDotNet.Model.Options.GetCardOptions;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Get a list of Trello Lists as a batch
        /// </summary>
        /// <param name="ids">List Ids</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Lists</returns>
        public async Task<List<List>> GetListsAsync(List<string> ids, CancellationToken cancellationToken = default)
        {
            return await ExecuteBatchedRequestAsync<List>(ids.Select(id => $"/{GetUrlBuilder.GetList(id)}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of cards as a batch
        /// </summary>
        /// <param name="ids">Card Ids</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Cards</returns>
        public async Task<List<Card>> GetCardsAsync(List<string> ids, CancellationToken cancellationToken = default)
        {
            return await ExecuteBatchedRequestAsync<Card>(ids.Select(id => $"/{GetUrlBuilder.GetCard(id)}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of cards as a batch
        /// </summary>
        /// <param name="ids">Card Ids</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Cards</returns>
        public async Task<List<Card>> GetCardsAsync(List<string> ids, GetCardOptions options, CancellationToken cancellationToken = default)
        {
            StringBuilder parametersAsString = ApiRequestController.GetParametersAsString(options.GetParameters(false)).Replace("&", "?", 0, 1);
            return await ExecuteBatchedRequestAsync<Card>(ids.Select(id => $"/{UrlPaths.Cards}/{id}{parametersAsString}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of boards as a batch
        /// </summary>
        /// <param name="ids">Board Ids</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Boards</returns>
        public async Task<List<Board>> GetBoardsAsync(List<string> ids, CancellationToken cancellationToken = default)
        {
            return await ExecuteBatchedRequestAsync<Board>(ids.Select(id => $"/{UrlPaths.Boards}/{id}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of boards as a batch
        /// </summary>
        /// <param name="ids">Board Ids</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Boards</returns>
        public async Task<List<Board>> GetBoardsAsync(List<string> ids, GetBoardOptions options, CancellationToken cancellationToken = default)
        {
            StringBuilder parametersAsString = ApiRequestController.GetParametersAsString(options.GetParameters()).Replace("&", "?", 0, 1);
            return await ExecuteBatchedRequestAsync<Board>(ids.Select(id => $"/{UrlPaths.Boards}/{id}{parametersAsString}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of members as a batch
        /// </summary>
        /// <param name="ids">Member Ids</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Members</returns>
        public async Task<List<Member>> GetMembersAsync(List<string> ids, CancellationToken cancellationToken = default)
        {
            return await ExecuteBatchedRequestAsync<Member>(ids.Select(id => $"/{UrlPaths.Members}/{id}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get a list of organizations as a batch
        /// </summary>
        /// <param name="ids">Organization Ids</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of Organizations</returns>
        public async Task<List<Organization>> GetOrganizationsAsync(List<string> ids, CancellationToken cancellationToken = default)
        {
            return await ExecuteBatchedRequestAsync<Organization>(ids.Select(id => $"/{UrlPaths.Organizations}/{id}").ToList(), cancellationToken);
        }

        /// <summary>
        /// Get batched data of the same type (T) as a list
        /// </summary>
        /// <typeparam name="T">The type to return (example Card or Board)</typeparam>
        /// <param name="urls">The REST API Suffix Urls (example: /boards/{boardId}/lists)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>The list of T</returns>
        /// <exception cref="TrelloApiException"></exception>
        private async Task<List<T>> ExecuteBatchedRequestAsync<T>(List<string> urls, CancellationToken cancellationToken = default)
        {
            BatchResultSummary batchResult = await GetBatchedRequestDataAsync(urls, cancellationToken);
            if (batchResult.ErrorCount == 0)
            {
                return batchResult.Results.Select(x => x.StatusCode == 200 ? x.GetData<T>() : default).ToList();
            }

            //Error scenario
            List<string> errors = new List<string>();
            foreach (BatchResultForUrl result in batchResult.Results)
            {
                if (result.StatusCode != 200)
                {
                    errors.Add($"'{result.Url}' resulted in error: {result.StatusCode}{result.Name} ({result.Message})");
                }
            }

            if (batchResult.ErrorCount > 0)
            {
                throw new TrelloApiException($"{batchResult.ErrorCount} of {batchResult.Results.Count} batched results failed: {string.Join(" | ", errors)}", string.Empty);
            }

            return batchResult.Results.Select(x => x.StatusCode == 200 ? x.GetData<T>() : default).ToList();
        }

        /// <summary>
        /// Make a generic Batch request of URLs of type GET data
        /// </summary>
        /// <param name="urls">The REST API Suffix Urls (example: /boards/{boardId}/lists)</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>a Batch-result</returns>
        private async Task<BatchResultSummary> GetBatchedRequestDataAsync(List<string> urls, CancellationToken cancellationToken = default)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            List<BatchResultForUrl> batchResultForUrls = new List<BatchResultForUrl>();
            while (batchResultForUrls.Count < urls.Count)
            {
                List<string> urlsToProcessThisBatch = urls.Skip(batchResultForUrls.Count).Take(10).ToList();
                List<string> preparedUrls = urlsToProcessThisBatch.Select(url => url.StartsWith("/") ? url : $"/{url}").ToList();
                string json = await _apiRequestController.Get("batch", cancellationToken, 0, new QueryParameter("urls", string.Join(",", preparedUrls)));
                List<BatchResultForUrl> results = JsonSerializer.Deserialize<List<BatchResultForUrl>>(json);
                //Attach URL to each result
                for (int i = 0; i < results.Count; i++)
                {
                    results[i].Url = urls[i + batchResultForUrls.Count];
                }

                batchResultForUrls.AddRange(results);
            }

            return new BatchResultSummary(batchResultForUrls);
        }

        /// <summary>
        /// Make a generic set of Batch request of type GET data
        /// </summary>
        /// <param name="requests">Request, each with a URL and an action for what to do with the result data</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        public async Task ExecuteBatchedRequestAsync(List<BatchRequest> requests, CancellationToken cancellationToken = default)
        {
            if (requests == null)
            {
                throw new ArgumentNullException(nameof(requests));
            }

            List<BatchResultForUrl> batchResultForUrls = new List<BatchResultForUrl>();
            while (batchResultForUrls.Count < requests.Count)
            {
                List<BatchRequest> urlsToProcessThisBatch = requests.Skip(batchResultForUrls.Count).Take(10).ToList();
                List<string> preparedUrls = urlsToProcessThisBatch.Select(x => x.Url.StartsWith("/") ? x.Url : $"/{x.Url}").ToList();
                string json = await _apiRequestController.Get("batch", cancellationToken, 0, new QueryParameter("urls", string.Join(",", preparedUrls)));
                List<BatchResultForUrl> results = JsonSerializer.Deserialize<List<BatchResultForUrl>>(json);

                //Attach URL to each result
                for (int i = 0; i < results.Count; i++)
                {
                    results[i].Url = requests[i + batchResultForUrls.Count].Url;
                }

                BatchResultSummary batchResult = new BatchResultSummary(results);
                if (batchResult.ErrorCount > 0)
                {
                    throw new Exception($"Error getting Trello data: {string.Join(" | ", batchResult.Errors)}");
                }

                for (int i = 0; i < results.Count; i++)
                {
                    requests[i + batchResultForUrls.Count].Action.Invoke(results[i]);
                }

                batchResultForUrls.AddRange(results);
            }
        }
    }
}