using System.Collections.Generic;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    /// <summary>
    /// Returns Master Data from KD
    /// </summary>
    public class SystemSeriesFactory : CoreDataFactory<ISystemSeriesRepository>, ISystemSeriesFactory
    {
        /// <summary>
        /// Constructor of work factory
        /// </summary>
        /// <param name="repositories"></param>
        public SystemSeriesFactory(IEnumerable<ISystemSeriesRepository> repositories) : base(repositories)
        {
        }

        /// <summary>
        /// Returns List of Series From KD
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="serieRequest"></param>
        /// <returns></returns>
        public GetSeriesResponse GetSeries(DataScope dataScope, GetSeriesRequest serieRequest)
        {
            return this[dataScope].GetSeries(serieRequest);
        }

        /// <summary>
        /// Returns list of Series
        /// </summary>
        /// <returns></returns>
        public Series GetSerieById(DataScope dataScope, int serieId)
        {
            return this[dataScope].GetSerieById(serieId);
        }
    }
}
