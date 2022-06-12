using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface ISystemSeriesRepository: ICoreDataRepository
    {
        GetSeriesResponse GetSeries(GetSeriesRequest serieRequest);

        Series GetSerieById(int serieId);
    }
}
