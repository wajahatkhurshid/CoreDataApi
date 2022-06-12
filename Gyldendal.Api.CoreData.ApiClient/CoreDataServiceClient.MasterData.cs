using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string MasterDataController = "v1/MasterData";

        /// <summary>
        /// Calls Core Data Service to get Area by Website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        public List<Area> GetAreas(WebShop webSite)
        {
            var queryString = $"{MasterDataController}/Areas/{webSite}";
            return HttpClient.GetAsync<List<Area>>(queryString);
        }

        /// <summary>
        /// Calls Core Data Service to get Subject according to website and according to area
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<Subject> GetSubjects(WebShop webSite, int areaId)
        {
            return HttpClient.GetAsync<List<Subject>>($"{MasterDataController}/Subjects/{webSite}/{areaId}");
        }

        /// <summary>
        /// Calls Core Data Service to get Sub Areas against the given web shop and the subject id.
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public List<SubArea> GetSubAreas(WebShop webSite, int subjectId)
        {
            return HttpClient.GetAsync<List<SubArea>>($"{MasterDataController}/SubAreas/{webSite}/{subjectId}");
        }

        /// <summary>
        /// Calls Core Data Service to get Levels according to website and according to area
        /// </summary>
        /// <param name="webSite"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public List<Level> GetLevels(WebShop webSite, int areaId)
        {
            return HttpClient.GetAsync<List<Level>>($"{MasterDataController}/Levels/{webSite}/{areaId}");
        }

        /// <summary>
        /// Calls Core Data Service to get MediaTypes according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        public List<MediaType> GetMediaTypes(WebShop webSite)
        {
            return HttpClient.GetAsync<List<MediaType>>($"{MasterDataController}/MediaTypes/{webSite}");
        }

        /// <summary>
        /// Calls Core Data Service to get MaterialTypes according to website
        /// </summary>
        /// <param name="webSite"></param>
        /// <returns></returns>
        public List<MaterialType> GetMaterialTypes(WebShop webSite)
        {
            var result = HttpClient.GetAsync<List<MaterialType>>($"{MasterDataController}/MaterialTypes/{webSite}");
            return result;
        }
    }
}
