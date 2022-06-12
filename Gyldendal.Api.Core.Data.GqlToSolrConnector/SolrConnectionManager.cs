using System;
using System.Collections.Generic;
using System.Linq;
using SolrNet;
using SolrNet.Impl;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector
{
    /// <summary>
    /// Manages a state whether a SolrConnection for a particuler set of Server Url and Core Name is been initialized or not.
    /// Initializes the connection if not already initialized.
    /// </summary>
    public static class SolrConnectionManager
    {
        private static readonly List<string> InitializedCoreUrls;

        private static readonly object SyncObject = new object();

        static SolrConnectionManager()
        {
            InitializedCoreUrls = new List<string>();
        }

        /// <summary>
        /// Initializes the SolrConnection if not already initialized for the given Server Url and Core Name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="solrServerUrl"></param>
        /// <param name="coreName"></param>
        public static void InitConnection<T>(string solrServerUrl, string coreName)
        {
            var coreUrl = solrServerUrl + coreName;

            // Don't lock and check if found return
            // ReSharper disable once InconsistentlySynchronizedField
            if (InitializedCoreUrls.Any(x => x.Equals(coreUrl, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            lock (SyncObject)
            {
                // check before insertion
                if (InitializedCoreUrls.Any(x => x.Equals(coreUrl, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                var connection = new SolrConnection(solrServerUrl + coreName);
                Startup.Init<T>(connection);

                InitializedCoreUrls.Add(coreUrl);
            }
        }
    }
}