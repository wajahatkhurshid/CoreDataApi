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

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
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
            base(DataScope.GyldendalPlus, kdEntities)
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
        /// Returns IQueryable of GPlus series db objec, for the query type i.e. SeriesOnly, SystemOnly, or All.
        /// </summary>
        /// <returns></returns>
        private IQueryable<DEA_KDWS_GPlusseries> GetSystemSeriesQuery(GetSeriesRequestType queryType)
        {
            var seriesQuery =
                KdEntities.DEA_KDWS_GPlusseries

                .Include("DEA_KDWS_GPlusserieAreas")
                .Include("DEA_KDWS_GPlusserieSubjects")
                .Include("DEA_KDWS_GPlusserieSubAreas")
                .Include("DEA_KDWS_GPlusseriesLevel.DEA_KDWS_GPluslevel")

                .Include("DEA_KDWS_GPlusseries2.DEA_KDWS_GPlusserieAreas")
                .Include("DEA_KDWS_GPlusseries2.DEA_KDWS_GPlusserieSubjects")
                .Include("DEA_KDWS_GPlusseries2.DEA_KDWS_GPlusserieSubAreas")
                .Include("DEA_KDWS_GPlusseries2.DEA_KDWS_GPlusseriesLevel.DEA_KDWS_GPluslevel")

                .Include("DEA_KDWS_GPlusseries1.DEA_KDWS_GPlusserieAreas")
                .Include("DEA_KDWS_GPlusseries1.DEA_KDWS_GPlusserieSubjects")
                .Include("DEA_KDWS_GPlusseries1.DEA_KDWS_GPlusserieSubAreas")
                .Include("DEA_KDWS_GPlusseries1.DEA_KDWS_GPlusseriesLevel.DEA_KDWS_GPluslevel")

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
        private static IQueryable<DEA_KDWS_GPlusseries> ApplyFilters(IQueryable<DEA_KDWS_GPlusseries> seriesQuery, GetSeriesRequest request)
        {
            var subjectFilter = !(string.IsNullOrWhiteSpace(request.Subject));
            var areaFilter = !(string.IsNullOrWhiteSpace(request.Area));

            if (!(subjectFilter) && !(areaFilter))
            {
                return seriesQuery;
            }

            if (areaFilter)
            {
                seriesQuery = seriesQuery.Where(x => x.DEA_KDWS_GPlusserieAreas.Any(y => y.area == request.Area));
            }

            if (subjectFilter)
            {
                seriesQuery = seriesQuery.Where(x => x.DEA_KDWS_GPlusserieSubjects.Any(y => y.subject == request.Subject));
            }

            return seriesQuery;
        }

        /// <summary>
        /// Applies the sort parameters from the GetSeriesRequest, to the provided series query.
        /// </summary>
        /// <param name="seriesQuery"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static IQueryable<DEA_KDWS_GPlusseries> ApplySorting(IQueryable<DEA_KDWS_GPlusseries> seriesQuery, GetSeriesRequest request)
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
        /// Materializes the IQueryable of GPlus series db object into domain model's Series object.
        /// </summary>
        /// <param name="seriesQuery"></param>
        /// <returns></returns>
        private IEnumerable<Series> MaterializeSeriesQuery(IQueryable<DEA_KDWS_GPlusseries> seriesQuery)
        {
            return seriesQuery.ToList().Select(s => ConstructSeries(s, true)).ToArray();
        }

        /// <summary>
        /// Construcsts a Series object using the given database object and supplimentary data.
        /// </summary>
        /// <param name="gPlusSeries"></param>
        /// <param name="setParentAndChild"></param>
        /// <returns></returns>
        private Series ConstructSeries(DEA_KDWS_GPlusseries gPlusSeries, bool setParentAndChild = false)
        {
            var retVal = new Series
            {
                ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(gPlusSeries.id, _configManager, gPlusSeries.Is_Image_Uploaded),
                Id = gPlusSeries.id,
                Name = gPlusSeries.navn,
                WebShop = WebShop,
                Description = gPlusSeries.beskrivelse,
                Url = gPlusSeries.webadresse,
                Areas = gPlusSeries.DEA_KDWS_GPlusserieAreas?.Select(x => new Area
                {
                    Id = x.AreaId,
                    Name = x.area,
                    WebShop = WebShop
                }).ToList(),
                Subjects = gPlusSeries.DEA_KDWS_GPlusserieSubjects?.Select(x => new Subject
                {
                    Id = x.SubjectId,
                    Name = x.subject,
                    AreaId = x.AreaId,
                    WebShop = WebShop
                }).ToList(),
                SubAreas = gPlusSeries.DEA_KDWS_GPlusserieSubAreas?.Select(x => new SubArea
                {
                    Id = x.SubAreaId,
                    Name = x.subarea,
                    SubjectId = x.subjectId,
                    WebShop = WebShop
                }).ToList(),
                Levels = gPlusSeries.DEA_KDWS_GPlusseriesLevel?.Select(l => new Level
                {
                    WebShop = WebShop,
                    Name = l.DEA_KDWS_GPluslevel.navn,
                    LevelNumber = l.DEA_KDWS_GPluslevel.niveau,
                    AreaId = l.DEA_KDWS_GPluslevel.kategori_id
                }).ToList(),
                LastUpdated = gPlusSeries.LastUpdated,
                IsSystemSeries = gPlusSeries.Type == 1,
                ParentSerieId = gPlusSeries.parent_id
            };

            if (setParentAndChild && gPlusSeries.DEA_KDWS_GPlusseries2 != null)
            {
                retVal.ParentSeries = ConstructSeries(gPlusSeries.DEA_KDWS_GPlusseries2);
            }

            if (setParentAndChild && (gPlusSeries.DEA_KDWS_GPlusseries1?.Any() ?? false))
            {
                retVal.ChildSeries = new List<Series>();
                foreach (var childSerie in gPlusSeries.DEA_KDWS_GPlusseries1)
                {
                    retVal.ChildSeries.Add(ConstructSeries(childSerie));
                }
            }

            return retVal;
        }
    }
}