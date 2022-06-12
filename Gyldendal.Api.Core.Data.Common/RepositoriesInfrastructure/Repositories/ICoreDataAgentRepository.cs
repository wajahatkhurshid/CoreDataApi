using Gyldendal.Api.CoreData.Common.Request;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface ICoreDataAgentRepository
    {
        object GetCoreDataAgentImportStates(ImportStates importStates);
    }
}