using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable UnusedMember.Global

namespace TrelloDotNet.Model.Batch
{
    /// <summary>
    /// Represent a Batch-result 
    /// </summary>
    internal class BatchResultSummary
    {
        /// <summary>
        /// The number of the URLs sent that resulted in failure (not 200 return)
        /// </summary>
        public int ErrorCount => Results.Count(x => x.StatusCode != 200);

        /// <summary>
        /// The errors of the URLs sent that resulted in failure (not 200 return)
        /// </summary>
        public List<string> Errors
        {
            get
            {
                List<string> errors = new List<string>();
                foreach (BatchResultForUrl result in Results)
                {
                    if (result.StatusCode != 200)
                    {
                        errors.Add($"'{result.Url}' resulted in error: {result.StatusCode}{result.Name} ({result.Message})");
                    }
                }

                return errors;
            }
        }

        /// <summary>
        /// The results of the BatchRequest
        /// </summary>
        public List<BatchResultForUrl> Results { get; set; }

        /// <summary>
        /// Get data for a specific request URL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public T GetDataForUrl<T>(string url)
        {
            BatchResultForUrl result = Results.SingleOrDefault(x => x.Url == url);
            if (result == null)
            {
                throw new ArgumentException("No data existing for this URL", nameof(url));
            }

            if (result.StatusCode != 200)
            {
                throw new ArgumentException($"Data-retrival for this url failed: {result.Name} - {result.Message}", nameof(url));
            }

            return result.GetData<T>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="results">The results of the BatchRequest</param>
        public BatchResultSummary(List<BatchResultForUrl> results)
        {
            Results = results;
        }
    }
}