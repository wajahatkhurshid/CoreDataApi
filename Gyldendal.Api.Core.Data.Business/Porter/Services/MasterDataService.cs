using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Util;
using CoreDataModels = Gyldendal.Api.CoreData.Contracts.Models;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class MasterDataService : IMasterDataService
    {
        PorterApi.IPorterClient _porterClient;
        public MasterDataService(PorterApi.IPorterClient porterClient)
        {
            _porterClient = porterClient;
        }

        public async Task<IEnumerable<CoreDataModels.MaterialType>> GetMaterialTypesAsync(WebShop webShop)
        {
            var materialTypesList = await _porterClient.MasterdataApiV1MasterdataMaterialtypesAsync();

            var materialTypes = materialTypesList.MaterialTypes.Select(x => new CoreDataModels.MaterialType
            {
                Name = x.Name,
                WebShop = webShop.ToPorterWebShop().ToCoreDataWebShop()
            }).ToList();

            return materialTypes;
        }

        public async Task<IEnumerable<CoreDataModels.MediaType>> GetMediaTypesAsync(WebShop webShop)
        {
            var mediaTypesList = await _porterClient.MasterdataApiV1MasterdataMediatypesAsync();

            var mediaTypes = mediaTypesList.MediaTypes.Select(x => new CoreDataModels.MediaType
            {
                Name = x.Name,
                WebShop = webShop.ToPorterWebShop().ToCoreDataWebShop()
            }).ToList();

            return mediaTypes;
        }

        public async Task<IEnumerable<CoreDataModels.Area>> GetAreasAsync(WebShop webShop)
        {
            var areasList = await _porterClient.MasterdataApiV1MasterdataAreasAsync(webShop.ToPorterWebShop());

            var areas = areasList.Areas.Select(x => new CoreDataModels.Area
            {
                Id = x.Id,
                Name = x.Name,
                WebShop = x.WebShop.ToCoreDataWebShop(),
            }).ToList();

            return areas;
        }

        public async Task<IEnumerable<CoreDataModels.Subject>> GetSubjectsAsync(WebShop webShop, int areaId)
        {
            var subjectsList = await _porterClient.MasterdataApiV1PortermasterdataSubjectsAsync(new PorterApi.GetSubjectsRequest{
                WebShop = webShop.ToPorterWebShop(), 
                AreaId = areaId != 0 ? areaId:null
                });

            var subjects = subjectsList.Subjects.Select(x => new CoreDataModels.Subject
            {
                Id = x.Id,
                Name = x.Name,
                WebShop = x.WebShop.ToCoreDataWebShop(),
                AreaId = x.AreaId
            }).ToList();

            return subjects;
        } 

        public async Task<IEnumerable<CoreDataModels.SubArea>> GetSubAreasAsync(WebShop webShop, int subjectId)
        {
            var subAreasList = await _porterClient.MasterdataApiV1PortermasterdataSubareasAsync(new PorterApi.GetSubAreasRequest{
                WebShop = webShop.ToPorterWebShop(), 
                SubjectId = subjectId != 0 ? subjectId:null
                });

            var subAreas = subAreasList.SubAreas.Select(x => new CoreDataModels.SubArea
            {
                Id = x.Id,
                Name = x.Name,
                WebShop = x.WebShop.ToCoreDataWebShop(),
                SubjectId = x.SubjectId
            }).ToList();

            return subAreas;
        } 

        public async Task<IEnumerable<CoreDataModels.Level>> GetLevelsAsync(WebShop webShop, int areaId)
        {
            var levelsList = await _porterClient.MasterdataApiV1MasterdataLevelsAsync(new PorterApi.GetLevelsRequest{
                WebShop = webShop.ToPorterWebShop(), 
                AreaId = areaId != 0 ? areaId:null
                });

            var levels = levelsList.Levels.Select(x => new CoreDataModels.Level
            {
                LevelNumber = x.LevelNumber,
                WebShop = x.WebShop.ToCoreDataWebShop(),
                AreaId = x.AreaId,
                Name = x.Name,
            }).ToList();

            return levels;
        } 
    }
}
