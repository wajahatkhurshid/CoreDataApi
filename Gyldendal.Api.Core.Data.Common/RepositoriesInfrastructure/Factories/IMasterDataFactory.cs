using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface IMasterDataFactory
    {
        /// <summary>
        /// Returns List of Media Types 
        /// </summary>
        /// <returns></returns>
        IEnumerable<MediaType> GetMediaTypes(DataScope dataScope);

        /// <summary>
        /// Returns List of Areas
        /// </summary>
        /// <returns></returns>
        IEnumerable<Area> GetAreas(DataScope dataScope);

        /// <summary>
        /// Returns List of Levels
        /// </summary>
        /// <returns></returns>
        IEnumerable<Level> GetLevels(DataScope dataScope, int areaId);

        /// <summary>
        /// Returns List of subjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<Subject> GetSubjects(DataScope dataScope, int areaId);

        /// <summary>
        /// Returns the list of Sub Areas agains the given web shop and subject id.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        IEnumerable<SubArea> GetSubAreas(DataScope dataScope, int subjectId);

        /// <summary>
        /// Returns List of MaterialTypes from KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        IEnumerable<MaterialType> GetMaterialTypes(DataScope dataScope);
    }
}
