using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlToSolrConnector.SolrUtils;
using Gyldendal.Api.CoreData.SolrDataProviders.Mappings;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Different Diagnostic methods for monitoring or internal use.
    /// </summary>
    [IsGdprSafe(true)]
    public class DiagnosticController : ApiController
    {
        private readonly Dictionary<GqlOperation, string> _gqlOpToSolrFieldMapping;

        private readonly ICoreDataAgentRepository _coreDataAgentRepository;

        /// <summary>
        ///
        /// </summary>
        /// <param name="coreDataAgentRepository"></param>
        public DiagnosticController(ICoreDataAgentRepository coreDataAgentRepository)
        {
            _gqlOpToSolrFieldMapping = GqlToSolrFieldMapping.GetMappings();
            _coreDataAgentRepository = coreDataAgentRepository;
        }

        /// <summary>
        /// Build a Solr query from provided gql.
        /// </summary>
        /// <param name="expression">Gql Expression</param>
        /// <param name="applyBoosting">This will not work for GeneralSearch or WorkSearch</param>
        /// <param name="useExpressionTree">This will not work for GeneralSearch or WorkSearch</param>
        /// <param name="dataScope">For GeneralSearch only</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Diagnostic/GqlToSolrQuery")]
        [ResponseType(typeof(string))]
        public string GqlToSolrQuery(string expression, bool applyBoosting, bool useExpressionTree, DataScope dataScope)
        {
            var solrQuery = new GqlToSolrQueryBuilder().Build(expression, applyBoosting, useExpressionTree, _gqlOpToSolrFieldMapping, dataScope);
            return solrQuery;
        }

        /// <summary>
        /// Get Core Data Agent Import State
        /// </summary>
        /// <param name="importState"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Diagnostic/GetCoreDataAgentImportState/{importState}")]
        public IHttpActionResult GetCoreDataAgentImportState(ImportStates importState)
        {
            var obj = _coreDataAgentRepository.GetCoreDataAgentImportStates(importState);
            return Ok(obj);
        }
    }
}