using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface IMasterDataRepository : ICoreDataRepository
    {
        IEnumerable<MediaType> GetMediaTypes();

        IEnumerable<Area> GetAreas();

        IEnumerable<Subject> GetSubjects(int areaId);

        IEnumerable<SubArea> GetSubAreas(int subjectId);

        IEnumerable<Level> GetLevels(int areaId);

        IEnumerable<MaterialType> GetMaterialTypes();
    }
}
