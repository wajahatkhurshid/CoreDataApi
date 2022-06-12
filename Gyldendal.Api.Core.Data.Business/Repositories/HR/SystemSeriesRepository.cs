using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.HR
{
    public class SystemSeriesRepository : BaseRepository, ISystemSeriesRepository
    {
        private readonly IConfigurationManager _configManager;

        /// <summary>
        /// Creates a new instance of SystemSeriesRepository.
        /// </summary>
        /// <param name="kdEntities"></param>
        /// <param name="configManager"></param>
        public SystemSeriesRepository(koncerndata_webshops_Entities kdEntities, IConfigurationManager configManager) :
            base(DataScope.HansReitzelShop, kdEntities)
        {
            _configManager = configManager;
        }

        /// <summary>
        /// Gets a paged response for a get series request.
        /// </summary>
        /// <returns></returns>
        public GetSeriesResponse GetSeries(GetSeriesRequest request)
        {
            var seriesQuery = GetSystemSeriesQuery(request.RequestType);

            seriesQuery = ApplyFilters(seriesQuery, request);

            var totalRecords = seriesQuery.Count();

            seriesQuery = ApplySorting(seriesQuery, request);

            seriesQuery = seriesQuery.Skip(request.PageSize * request.PageIndex).Take(request.PageSize).AsQueryable();

            var seriesList = MaterializeSeriesQuery(seriesQuery).ToList();

            return new GetSeriesResponse
            {
                Count = totalRecords,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Series = seriesList,
            };
        }

        /// <summary>
        /// Returns the Series object for the Id passed.
        /// </summary>
        /// <param name="serieId"></param>
        /// <returns></returns>
        public Series GetSerieById(int serieId)
        {
            var seriesQuery = GetSystemSeriesQuery(GetSeriesRequestType.All).Where(x => x.id == serieId);

            var pageResult = MaterializeSeriesQuery(seriesQuery);

            return pageResult.FirstOrDefault();
        }

        /// <summary>
        /// Returns IQueryable of HR series db objec, for the query type i.e. SeriesOnly, SystemOnly, or All.
        /// </summary>
        /// <returns></returns>
        private IQueryable<DEA_KDWS_HRseries> GetSystemSeriesQuery(GetSeriesRequestType queryType)
        {
            var seriesQuery =
                KdEntities.DEA_KDWS_HRseries

                .Include("DEA_KDWS_HRserieAreas")
                .Include("DEA_KDWS_HRserieSubjects")
                .Include("DEA_KDWS_HRserieSubAreas")
                .Include("DEA_KDWS_HRseriesLevel.DEA_KDWS_HRlevel")

                .Include("DEA_KDWS_HRseries2.DEA_KDWS_HRserieAreas")
                .Include("DEA_KDWS_HRseries2.DEA_KDWS_HRserieSubjects")
                .Include("DEA_KDWS_HRseries2.DEA_KDWS_HRserieSubAreas")
                .Include("DEA_KDWS_HRseries2.DEA_KDWS_HRseriesLevel.DEA_KDWS_HRlevel")

                .Include("DEA_KDWS_HRseries1.DEA_KDWS_HRserieAreas")
                .Include("DEA_KDWS_HRseries1.DEA_KDWS_HRserieSubjects")
                .Include("DEA_KDWS_HRseries1.DEA_KDWS_HRserieSubAreas")
                .Include("DEA_KDWS_HRseries1.DEA_KDWS_HRseriesLevel.DEA_KDWS_HRlevel")

                .AsQueryable();

            if (queryType == GetSeriesRequestType.SystemSeriesOnly)
            {
                seriesQuery = seriesQuery.Where(x => x.Type == 1);
            }
            else if (queryType == GetSeriesRequestType.SeriesOnly)
            {
                seriesQuery = seriesQuery.Where(x => x.Type == 0);
            }

            return seriesQuery;
        }

        /// <summary>
        /// Applies the Subject and Serie filters on the IQueryable of database series objects.
        /// </summary>
        /// <param name="seriesQuery"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IQueryable<DEA_KDWS_HRseries> ApplyFilters(IQueryable<DEA_KDWS_HRseries> seriesQuery, GetSeriesRequest request)
        {
            var subjectFilter = !(string.IsNullOrWhiteSpace(request.Subject));
            var areaFilter = !(string.IsNullOrWhiteSpace(request.Area));

            if (!(subjectFilter) && !(areaFilter))
            {
                return seriesQuery;
            }

            if (areaFilter)
            {
                seriesQuery = seriesQuery.Where(x => x.DEA_KDWS_HRserieAreas.Any(y => y.area == request.Area));
            }

            if (subjectFilter)
            {
                seriesQuery = seriesQuery.Where(x => x.DEA_KDWS_HRserieSubjects.Any(y => y.subject == request.Subject));
            }

            return seriesQuery;
        }

        /// <summary>
        /// Applies the sort parameters from the GetSeriesRequest, to the provided series query.
        /// </summary>
        /// <param name="seriesQuery"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IQueryable<DEA_KDWS_HRseries> ApplySorting(IQueryable<DEA_KDWS_HRseries> seriesQuery, GetSeriesRequest request)
        {
            switch (request.OrderBy)
            {
                case SeriesOrderBy.Name:
                    seriesQuery = request.SortBy == SortBy.Desc
                        ? seriesQuery.OrderByDescending(x => x.navn)
                        : seriesQuery.OrderBy(x => x.navn);
                    break;

                case SeriesOrderBy.LastUpdated:
                    seriesQuery = request.SortBy == SortBy.Desc
                        ? seriesQuery.OrderByDescending(x => x.LastUpdated)
                        : seriesQuery.OrderBy(x => x.LastUpdated);
                    break;

                default:
                    seriesQuery = request.SortBy == SortBy.Desc
                        ? seriesQuery.OrderByDescending(x => x.navn)
                        : seriesQuery.OrderBy(x => x.navn);
                    break;
            }

            return seriesQuery;
        }

        /// <summary>
        /// Materializes the IQueryable of HR series db object into domain model's Series object.
        /// </summary>
        /// <param name="seriesQuery"></param>
        /// <returns></returns>
        private IEnumerable<Series> MaterializeSeriesQuery(IQueryable<DEA_KDWS_HRseries> seriesQuery)
        {
            return seriesQuery.ToList().Select(s => ConstructSeries(s, true)).ToArray();
        }

        /// <summary>
        /// Construcsts a Series object using the given database object and supplimentary data.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="setParentAndChild"></param>
        /// <returns></returns>
        private Series ConstructSeries(DEA_KDWS_HRseries series, bool setParentAndChild = false)
        {
            var retVal = new Series
            {
                ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(series.id, _configManager, series.Is_Image_Uploaded),
                Id = series.id,
                Name = series.navn,
                WebShop = WebShop,
                Description = series.beskrivelse,
                Url = series.webadresse,
                Areas = series.DEA_KDWS_HRserieAreas?.Select(x => new Area
                {
                    Id = x.AreaId,
                    Name = x.area,
                    WebShop = WebShop
                }).ToList(),
                Subjects = series.DEA_KDWS_HRserieSubjects?.Select(x => new Subject
                {
                    Id = x.SubjectId,
                    AreaId = x.AreaId,
                    Name = x.subject,
                    WebShop = WebShop
                }).ToList(),
                SubAreas = series.DEA_KDWS_HRserieSubAreas?.Select(x => new SubArea
                {
                    Id = x.SubAreaId,
                    Name = x.subarea,
                    SubjectId = x.subjectId,
                    WebShop = WebShop
                }).ToList(),
                Levels = series.DEA_KDWS_HRseriesLevel?.Select(l => new Level
                {
                    WebShop = WebShop,
                    Name = l.DEA_KDWS_HRlevel.navn,
                    LevelNumber = l.DEA_KDWS_HRlevel.niveau,
                    AreaId = l.DEA_KDWS_HRlevel.kategori_id
                }).ToList(),
                LastUpdated = series.LastUpdated,
                IsSystemSeries = series.Type == 1,
                ParentSerieId = series.parent_id
            };

            if (setParentAndChild && series.DEA_KDWS_HRseries2 != null)
            {
                retVal.ParentSeries = ConstructSeries(series.DEA_KDWS_HRseries2);
            }

            if (setParentAndChild && (series.DEA_KDWS_HRseries1?.Any() ?? false))
            {
                retVal.ChildSeries = new List<Series>();
                foreach (var childSerie in series.DEA_KDWS_HRseries1)
                {
                    retVal.ChildSeries.Add(ConstructSeries(childSerie));
                }
            }

            return retVal;
        }
    }
}