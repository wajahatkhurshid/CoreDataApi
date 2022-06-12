using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Common.WebUtils.Exceptions;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    public class CoreDataFactory<T> where T : ICoreDataRepository
    {
        private readonly IEnumerable<T> _coreDataRepositories;

        protected CoreDataFactory(IEnumerable<T> coreDataRepositories)
        {
            _coreDataRepositories = coreDataRepositories;
        }
        
        protected T this[DataScope dataScope]
        {
            get
            {
                var repository = _coreDataRepositories.FirstOrDefault(r => r.DataScope == dataScope);
                if (repository != null)
                    return repository;
                throw new ProcessException((ulong)ErrorCodes.InvalidWebSite, ErrorCodes.InvalidWebSite.GetDescription(), Extensions.CoreDataSystemName);
            }
        }
    }
}
