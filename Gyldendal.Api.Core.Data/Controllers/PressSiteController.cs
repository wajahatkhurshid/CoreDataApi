using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Returns Press Site Related Data
    /// </summary>
    public class PressSiteController : ApiController
    {
        private readonly IPressSiteProvider _pressSiteProvider;

        private readonly IPressSiteService _pressSiteService;

        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;

        /// <param name="pressSiteProvider"></param>
        public PressSiteController(IPressSiteProvider pressSiteProvider, IPressSiteService pressSiteService, IConfigurationManager configurationManager)
        {
            _pressSiteProvider = pressSiteProvider;
            _pressSiteService = pressSiteService;
            _isShadowMode = configurationManager.IsShadowMode;
        }

        /// <summary>
        /// GetBogObject
        /// Returns book details  based on the Isbn13 number
        /// </summary>
        /// <param name="Isbn13"></param>
        /// <returns>Product Details</returns>
        [HttpGet]
        [Route("api/v1/PressSite/GetBook/{Isbn13}")]
        [ResponseType(typeof(PressSiteProduct))]
        public async Task<IHttpActionResult> GetBook(string Isbn13)
        {
            if (!_isShadowMode)
            {

                var result = _pressSiteProvider.GetBook(Isbn13);
                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetBookAsync(Isbn13);
                return Ok(result);

            }
        }

        /// <summary>
        /// GetForFatterTable
        /// returns list of authors and count of authors
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetAuthors")]
        [ResponseType(typeof(ForfatterResponse))]
        public async Task<IHttpActionResult> GetAuthors(AuthorsRequest request)
        {
            if (!_isShadowMode)
            {

                var result = _pressSiteProvider.GetAuthors(request);
                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetAuthorsAsync(request);
                return Ok(result);
            }
        }


        /// <summary>
        /// GetForfatterFotos
        /// returns author photos and count based on date range and searchstring
        /// </summary>
        ///  <param name="request></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetAuthorPhotos")]
        [ResponseType(typeof(ForfatterfotoResponse))]
        public async Task<IHttpActionResult> GetAuthorPhotos(AuthorPhotosRequest request)
        {
            if (!_isShadowMode)
            {
                var result = _pressSiteProvider.GetAuthorPhotos(request);
                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetAuthorPhotosAsync(request);
                return Ok(result);
            }
        }

        /// <summary>
        /// GetBogForsiderTable
        /// returns book covers based on date range searchstring and gener
        /// </summary>
        /// <param name="requset"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetBookCovers/")]
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> GetBookCovers(BogForsiderRequset requset)
        {
            if (!_isShadowMode)
            {

                var result = _pressSiteProvider.GetBookCovers(requset);

                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetProductsAsync(requset);
                return Ok(result);

            }

        }

        /// <summary>
        /// return news letter based on date range ,gener and options
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetNewsletterTitles/")]
        [ResponseType(typeof(List<PressSiteProduct>))]
        public async Task<IHttpActionResult> GetNewsletterTitles(NewsletterRequest request)
        {
            if (!_isShadowMode)
            {

                var result = _pressSiteProvider.GetNewsletterTitles(request);

                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetNewsletterTitles(request);
                return Ok(result);

            }
        }

        /// <summary>
        /// GetKommendeTitleTable
        ///  returns all the upcoming titles accoring to the search criteria
        /// </summary>
        /// <param name="requset"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetUpcomingTitles")]
        [ResponseType(typeof(ProductResponse))]
        public async Task<IHttpActionResult> GetUpcomingTitles(UpcomingTitlesRequest requset)
        {
            if (!_isShadowMode)
            {
                var result = _pressSiteProvider.GetUpcomingTitles(requset);

                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetUpcomingTitles(requset);
                return Ok(result);

            }
        }


        /// <summary>
        /// returns all the categories
        /// </summary>
        [HttpGet]
        [Route("api/v1/PressSite/GetCategories")]
        [ResponseType(typeof(List<string>))]
        public async Task<IHttpActionResult> GetCategories()
        {
            var result = _pressSiteProvider.GetCategories();

            return Ok(result);
        }

        /// <summary>
        /// returns all the books based on date range
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetProductsForImport/")]
        [ResponseType(typeof(List<PressSiteProduct>))]
        public async Task<IHttpActionResult> GetProductsForImport(ProductImportRequest request)
        {
            if (!_isShadowMode)
            {

                var result = _pressSiteProvider.GetProductsForImport(request);
                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetProductsForImport(request);
                return Ok(result);

            }

        }

        /// <summary>
        /// returns all the authors
        /// </summary>
        [HttpGet]
        [Route("api/v1/PressSite/GetAuthorsForImport")]
        [ResponseType(typeof(List<Forfatter>))]
        public async Task<IHttpActionResult> GetAuthorsForImport()
        {
            if (!_isShadowMode)
            {
                var result = _pressSiteProvider.GetAuthorsForImport();

                return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetAuthorsForImportAsync();
                return Ok(result);
            }

        }

        /// <summary>
        /// return list of books,titles and count of books , titles and authors
        /// </summary>
        /// <param name="request"></param>
        [HttpPost]
        [Route("api/v1/PressSite/GetTypeAheadResults/")]
        [ResponseType(typeof(TypeAheadResponse))]
        public async Task<IHttpActionResult> GetTypeAheadResults(TypeAheadRequest request)
        {
            if (!_isShadowMode)
            {
                var result = _pressSiteProvider.GetTypeAheadResults(request);
            return Ok(result);
            }
            else
            {
                var result = await _pressSiteService.GetTypeAheadResultsAsync(request);
                return Ok(result);
            }
        }

    }
}
