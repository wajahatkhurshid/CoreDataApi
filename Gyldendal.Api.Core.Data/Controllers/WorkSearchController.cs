using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Filters;
using Gyldendal.Api.CoreData.Gql.Common;
using Gyldendal.Api.CoreData.GqlValidator;
using Gyldendal.Common.WebUtils.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.ResultsPostProcessing.Infrastructure;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Contains action methods for Gql based search.
    /// </summary>
    [IsGdprSafe(true)]
    public class WorkSearchController : ApiController
    {
        private readonly IWorkDataProvider _workDataProvider;

        private readonly IWorksResultProcessesExecutor _worksResultProcessesExecutor;

        /// <param name="workDataProvider"></param>
        /// <param name="worksResultProcessesExecutor"></param>
        public WorkSearchController(IWorkDataProvider workDataProvider, IWorksResultProcessesExecutor worksResultProcessesExecutor)
        {
            _workDataProvider = workDataProvider;
            _worksResultProcessesExecutor = worksResultProcessesExecutor;
        }

        /// <summary>
        /// Returns a list of Work objects based on the search request criteria (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [NullValueFilter]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/v1/WorkSearch/Search")]
        [ResponseType(typeof(SearchResponse<Work>))]
        public IHttpActionResult SearchWorks(WorkSearchRequest request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) && !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();

            var response = _workDataProvider.Get(searchRequest);

            _worksResultProcessesExecutor.Execute(response, request.Webshop);

            return Ok(response);
        }

        /// <summary>
        /// Returns a list of Work objects based on the search request criteria (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [NullValueFilter]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/v2/WorkSearch/Search")]
        [ResponseType(typeof(SearchResponse<Work>))]
        public IHttpActionResult SearchWorksV2(WorkSearchRequestV2 request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) && !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();

            var response = _workDataProvider.Get(searchRequest);

            _worksResultProcessesExecutor.Execute(response, request.Webshop);

            return Ok(response);
        }

        /// <summary>
        /// Returns a list of Work objects based on the search request criteria (Source: Solr)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [NullValueFilter]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/v3/WorkSearch/Search")]
        [ResponseType(typeof(SearchResponse<Work>))]
        public IHttpActionResult SearchWorksV3(WorkSearchRequestV3 request)
        {
            if (!(string.IsNullOrWhiteSpace(request.Gql)) && !(new ExpressionValidator().Validate(request.Gql)).Result.IsValidated)
            {
                throw new ValidationException((ulong)ErrorCodes.InvalidGql, ErrorCodes.InvalidGql.GetDescription(), Extensions.CoreDataSystemName, null);
            }

            var searchRequest = request.ToWorkProductSearchRequest();

            var response = _workDataProvider.Get(searchRequest);

            _worksResultProcessesExecutor.Execute(response, request.CallingWebShop);

            return Ok(response);
        }

        /// <summary>
        /// Validates a Gql expression and sends an appropriate response.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/v1/WorkSearch/ValidateGqlExpression")]
        [ResponseType(typeof(ValidationResult))]
        public HttpResponseMessage ValidateExpression(string expression)
        {
            try
            {
                var jsonData = new JsonResult
                {
                    Data = new ExpressionValidator().Validate(expression).Result,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
                return Request.CreateResponse(HttpStatusCode.OK, jsonData);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}