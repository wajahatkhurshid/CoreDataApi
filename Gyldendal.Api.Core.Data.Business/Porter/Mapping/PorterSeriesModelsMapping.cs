using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Common.Utils;
using Area = Gyldendal.Api.CoreData.Contracts.Models.Area;
using Level = Gyldendal.Api.CoreData.Contracts.Models.Level;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Series = Gyldendal.Api.CoreData.Contracts.Models.Series;
using SubArea = Gyldendal.Api.CoreData.Contracts.Models.SubArea;
using Subject = Gyldendal.Api.CoreData.Contracts.Models.Subject;

namespace Gyldendal.Api.CoreData.Business.Porter.Mapping
{
    public static class PorterSeriesModelsMapping
    {
        /// <summary>
        /// Creates Series Object using Porter Series Object
        /// </summary>
        /// <param name="porterSeries"></param>
        /// <returns>Series</returns>
        internal static Series ToCoreDataSeries(this PorterApi.Series porterSeries)
        {
            return new Series
            {
                WebShop = porterSeries.WebShop.ToCoreDataWebShop(),
                Id = porterSeries.Id.ToInt(),
                Name = porterSeries.Name,
                Description = porterSeries.Description,
                Url = porterSeries.Url,
                ParentSerieId = porterSeries.ParentSerieId,
                ParentSeries = porterSeries.ParentSeries?.ToCoreDataSeries(),
                ChildSeries = porterSeries.ChildSeries.ToCoreDataSeriesList(),
                Areas = porterSeries.GetAreas(),
                SubAreas = porterSeries.GetSubAreas(),
                Levels = porterSeries.GetEducationLevels(),
                Subjects = porterSeries.GetSubjects(),
                ImageUrl = porterSeries.ImageUrl,
                LastUpdated = porterSeries.UpdatedTimestamp,
                IsSystemSeries = porterSeries.IsSystemSeries,
            };
        }

        /// <summary>
        /// Gets the List of Series using Porter Series List 
        /// </summary>
        /// <param name="seriesList"></param>
        /// <returns></returns>
        internal static List<Series> ToCoreDataSeriesList(this ICollection<PorterApi.Series> seriesList)
        {
            return seriesList.Select(x => new Series
            {
                Id = x.Id.ToInt(),
                Name = x.Name,
                Description = x.Description,
                Url = x.Url,
                ParentSeries = x.ParentSeries.ToCoreDataSeries(),
                ParentSerieId = x.ParentSerieId,
                ChildSeries = x.ChildSeries.ToCoreDataSeriesList(),
                Areas = x.GetAreas(),
                SubAreas = x.GetSubAreas(),
                Levels = x.GetEducationLevels(),
                Subjects = x.GetSubjects(),
                WebShop = x.WebShop.ToCoreDataWebShop(),
                LastUpdated = x.UpdatedTimestamp,
                ImageUrl = x.ImageUrl,
                IsSystemSeries = x.IsSystemSeries,
            }).ToList();
        }

        /// <summary>
        /// Gets the List of Areas using Porter Series Object
        /// </summary>
        /// <param name="porterSeries"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this PorterApi.Series porterSeries)
        {
            return porterSeries.Areas
                .Select(x => new Area
                {
                    Id = x.Id,
                    Name = x.Name,
                    WebShop = x.WebShop.ToCoreDataWebShop()
                }).ToList();
        }

        /// <summary>
        /// Gets the List of SubAreas using Porter Series Object
        /// </summary>
        /// <param name="porterSeries"></param>
        /// <returns></returns>
        private static List<SubArea> GetSubAreas(this PorterApi.Series porterSeries)
        {
            return porterSeries.SubAreas
                .Select(x => new SubArea
                {
                    Id = x.Id,
                    Name = x.Name,
                    WebShop = x.WebShop.ToCoreDataWebShop(),
                    SubjectId = x.SubjectId
                }).ToList();
        }

        /// <summary>
        /// Gets the List of Level using Porter Series Object
        /// </summary>
        /// <param name="porterSeries"></param>
        /// <returns></returns>
        private static List<Level> GetEducationLevels(this PorterApi.Series porterSeries)
        {
            return porterSeries.EducationLevels
                .Select(x => new Level
                {
                    LevelNumber = x.LevelNumber,
                    WebShop = x.WebShop.ToCoreDataWebShop(),
                    AreaId = x.AreaId,
                    Name = x.Name
                }).ToList();
        }

        /// <summary>
        /// Gets the List of Subject using Porter Series Object
        /// </summary>
        /// <param name="porterSeries"></param>
        /// <returns></returns>
        private static List<Subject> GetSubjects(this PorterApi.Series porterSeries)
        {
            return porterSeries.Subjects
                .Select(x => new Subject
                {
                    Id = x.Id,
                    WebShop = x.WebShop.ToCoreDataWebShop(),
                    AreaId = x.AreaId,
                    Name = x.Name
                }).ToList();
        }
    }
}