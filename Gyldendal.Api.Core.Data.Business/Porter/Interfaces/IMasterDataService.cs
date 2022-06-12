using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IMasterDataService 
    {
        Task<IEnumerable<MediaType>> GetMediaTypesAsync(WebShop webShop);

        Task<IEnumerable<MaterialType>> GetMaterialTypesAsync(WebShop webShop);

        Task<IEnumerable<Area>> GetAreasAsync(WebShop webShop);

        Task<IEnumerable<Subject>> GetSubjectsAsync(WebShop webShop, int areaId);

        Task<IEnumerable<SubArea>> GetSubAreasAsync(WebShop webShop, int subjectId);

        Task<IEnumerable<Level>> GetLevelsAsync(WebShop webShop, int areaId);
    }
}
