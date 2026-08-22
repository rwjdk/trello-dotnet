using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrelloDotNet.Model;
using TrelloDotNet.Model.Search;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet
{
    public partial class TrelloClient
    {
        /// <summary>
        /// Search Trello for Cards, Boards, and/or Organizations
        /// </summary>
        /// <remarks>
        /// Search-tips: https://blog.trello.com/the-secrets-of-superior-trello-searches
        /// </remarks>
        /// <param name="searchRequest">The Search Request</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The Search-result</returns>
        public async Task<SearchResult> SearchAsync(SearchRequest searchRequest, CancellationToken cancellationToken = default)
        {
            List<string> modelTypes = new List<string>();
            if (searchRequest.SearchOrganizations)
            {
                modelTypes.Add("organizations");
            }

            if (searchRequest.SearchBoards)
            {
                modelTypes.Add("boards");
            }

            if (searchRequest.SearchCards)
            {
                modelTypes.Add("cards");
            }


            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("query", searchRequest.SearchTerm),
                new QueryParameter("partial", searchRequest.PartialSearch),
                new QueryParameter("modelTypes", string.Join(",", modelTypes))
            };

            #region Boards-options

            if (searchRequest.BoardFilter != null)
            {
                parameters.Add(new QueryParameter("idBoards", string.Join(",", searchRequest.BoardFilter.BoardIds)));
            }

            if (searchRequest.BoardFields != null)
            {
                parameters.Add(new QueryParameter("board_fields", string.Join(",", searchRequest.BoardFields.FieldNames)));
            }

            if (searchRequest.BoardLimit.HasValue)
            {
                parameters.Add(new QueryParameter("boards_limit", searchRequest.BoardLimit.Value));
            }

            #endregion

            #region Organization-options

            if (searchRequest.OrganizationFilter != null)
            {
                parameters.Add(new QueryParameter("idOrganizations", string.Join(",", searchRequest.OrganizationFilter.OrganizationIds)));
            }

            #endregion

            #region Card-options

            if (searchRequest.CardFilter != null)
            {
                parameters.Add(new QueryParameter("idCards", string.Join(",", searchRequest.CardFilter.CardIds)));
            }

            if (searchRequest.CardFields != null)
            {
                parameters.Add(new QueryParameter("card_fields", string.Join(",", searchRequest.CardFields.FieldNames)));
            }

            if (searchRequest.CardLimit.HasValue)
            {
                parameters.Add(new QueryParameter("cards_limit", searchRequest.CardLimit.Value));
            }

            if (searchRequest.CardPage.HasValue)
            {
                parameters.Add(new QueryParameter("cards_page", searchRequest.CardPage.Value));
            }

            #endregion

            return await _apiRequestController.Get<SearchResult>(UrlPaths.Search, cancellationToken, parameters.ToArray());
        }

        /// <summary>
        /// Search Members across Trello
        /// </summary>
        /// <param name="searchRequest">The Search-request</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The List of Members from the search</returns>
        public async Task<List<Member>> SearchMembersAsync(SearchMemberRequest searchRequest, CancellationToken cancellationToken = default)
        {
            List<QueryParameter> parameters = new List<QueryParameter>
            {
                new QueryParameter("query", searchRequest.SearchTerm),
                new QueryParameter("onlyOrgMembers", searchRequest.OnlyOrgMembersFilter)
            };

            if (searchRequest.Limit.HasValue)
            {
                parameters.Add(new QueryParameter("limit", searchRequest.Limit.Value));
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.BoardIdFilter))
            {
                parameters.Add(new QueryParameter("idBoard", searchRequest.BoardIdFilter));
            }

            if (!string.IsNullOrWhiteSpace(searchRequest.OrganizationIdFilter))
            {
                parameters.Add(new QueryParameter("idOrganization", searchRequest.OrganizationIdFilter));
            }

            return await _apiRequestController.Get<List<Member>>($"{UrlPaths.Search}/{UrlPaths.Members}", cancellationToken, parameters.ToArray());
        }
    }
}