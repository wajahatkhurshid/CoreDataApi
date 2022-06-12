using System.Collections.Generic;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    /// <summary>
    /// Returns Master Data from KD
    /// </summary>
    public class MasterDataFactory : CoreDataFactory<IMasterDataRepository>, IMasterDataFactory
    {
        /// <summary>
        /// Constructor of work factory
        /// </summary>
        /// <param name="repositories"></param>
        public MasterDataFactory(IEnumerable<IMasterDataRepository> repositories) : base(repositories)
        {

        }

        /// <summary>
        /// Returns list of Media Types From KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        public IEnumerable<MediaType> GetMediaTypes(DataScope dataScope)
        {
            return this[dataScope].GetMediaTypes();
        }

        /// <summary>
        /// Returns list of Areas From KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        public IEnumerable<Area> GetAreas(DataScope dataScope)
        {
            return this[dataScope].GetAreas();
        }

        /// <summary>
        /// Returns List of Subjects From KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public IEnumerable<Subject> GetSubjects(DataScope dataScope, int areaId)
        {
            return this[dataScope].GetSubjects(areaId);
        }

        /// <summary>
        /// Returns a list of SubAreas against the given web shop and subject id.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public IEnumerable<SubArea> GetSubAreas(DataScope dataScope, int subjectId)
        {
            return this[dataScope].GetSubAreas(subjectId);
        }

        /// <summary>
        /// Returns list of Levels From KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        public IEnumerable<Level> GetLevels(DataScope dataScope, int areaId)
        {
            return this[dataScope].GetLevels(areaId);
        }

        /// <summary>
        /// Returns List of MaterialTypes from KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        public IEnumerable<MaterialType> GetMaterialTypes(DataScope dataScope)
        {
            return this[dataScope].GetMaterialTypes();
        }
    }
}
