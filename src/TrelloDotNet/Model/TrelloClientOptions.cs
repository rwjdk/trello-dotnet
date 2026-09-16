namespace TrelloDotNet.Model
{
    /// <summary>
    /// Options for the Trello Client
    /// </summary>
    public class TrelloClientOptions
    {
        /// <summary>
        /// Control level of URL Details are shown in Exceptions from calls to the API
        /// </summary>
        public ApiCallExceptionOption ApiCallExceptionOption { get; set; }

        /// <summary>
        /// Controls if it is allowed to delete Boards (secondary confirmation)
        /// </summary>
        public bool AllowDeleteOfBoards { get; set; }

        /// <summary>
        /// Controls if it is allowed to delete Organizations (secondary confirmation)
        /// </summary>
        public bool AllowDeleteOfOrganizations { get; set; }

        /// <summary>
        /// Controls how many automated Retries the API should try in case if you get an 'API_TOKEN_LIMIT_EXCEEDED' error from Trello (Default 3) set to -1 to disable the system
        /// </summary>
        public int MaxRetryCountForTokenLimitExceeded { get; set; }

        /// <summary>
        /// Controls how long in seconds system should wait between retries, should it receive an 'API_TOKEN_LIMIT_EXCEEDED' error from Trello (Default 1 sec)
        /// </summary>
        public double DelayInSecondsToWaitInTokenLimitExceededRetry { get; set; }

        /// <summary>
        /// Trello Auth application secret or confidential OAuth 2.0 client secret used to validate Webhook signatures
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Controls whether credentials are sent via query string or header
        /// </summary>
        public SendCredentialsMode SendCredentialsMode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public TrelloClientOptions()
        {
            ApiCallExceptionOption = ApiCallExceptionOption.IncludeUrlButMaskCredentials;
            AllowDeleteOfBoards = false;
            AllowDeleteOfOrganizations = false;
            MaxRetryCountForTokenLimitExceeded = 3;
            DelayInSecondsToWaitInTokenLimitExceededRetry = 1;
            Secret = null;
            SendCredentialsMode = TrelloDotNet.SendCredentialsMode.QueryString;
        }
    }
}
