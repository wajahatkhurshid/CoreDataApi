using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Repositories.Global
{
    public class BaseWorkProductRepository : BaseRepository
    {
        /// <summary>
        /// Creates a new instance of baseWorkProductRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        protected BaseWorkProductRepository(koncerndata_webshops_Entities kdEntities) :
            base(DataScope.Global, kdEntities)
        {
        }
    }
}