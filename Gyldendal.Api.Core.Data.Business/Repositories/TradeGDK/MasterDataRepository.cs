using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class MasterDataRepository : BaseRepository, IMasterDataRepository
    {
        public MasterDataRepository(koncerndata_webshops_Entities kdEntities) :
            base(DataScope.TradeGyldendalDk, kdEntities)
        {
        }

        public IEnumerable<MediaType> GetMediaTypes()
        {
            var mediaTypes = KdEntities.DEA_KDWS_GUmedietype
                                         .Where(x => x.website_medietype != null)
                                         .Select(x => x.website_medietype).Distinct().ToArray().Select(x => new MediaType
                                         {
                                             Name = x,
                                             WebShop = WebShop
                                         }).ToList();
            return mediaTypes;
        }

        public IEnumerable<Area> GetAreas()
        {
            var areas = KdEntities.DEA_KDWS_GDKcategory
                                   .Where(x => x.niveau == 1)
                                   .Select(a => new Area
                                   {
                                       Id = a.id,
                                       Name = a.navn.Trim(),
                                       WebShop = WebShop
                                   })
                                   .ToList();
            return areas;
        }

        public IEnumerable<Subject> GetSubjects(int areaId)
        {
            var subjects = KdEntities.DEA_KDWS_GDKcategory
                       .Where(x => x.niveau == 2 && (areaId == 0 || x.parent == areaId))
                       .Select(a => new Subject
                       {
                           Id = a.id,
                           Name = a.navn.Trim(),
                           WebShop = WebShop,
                           AreaId = a.parent
                       })
                       .ToList();
            return subjects;
        }

        public IEnumerable<SubArea> GetSubAreas(int subjectId)
        {
            var subAreas =
                           KdEntities.DEA_KDWS_GDKcategory
                           .Where(x => x.niveau == 3 && (subjectId == 0 || x.parent == subjectId))
                           .Select(a => new SubArea
                           {
                               Id = a.id,
                               Name = a.navn.Trim(),
                               WebShop = WebShop,
                               SubjectId = subjectId
                           })
                           .ToList();

            return subAreas;
        }

        public IEnumerable<Level> GetLevels(int areaId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MaterialType> GetMaterialTypes()
        {
            var materialTypes = KdEntities.DEA_KDWS_GUmaterialtype
                        .Where(m => m.website_materialtype != null)
                        .Select(x => x.website_materialtype).Distinct().ToArray().Select(x => new MaterialType
                        {
                            Name = x,
                            WebShop = WebShop
                        }).ToList();
            return materialTypes;
        }
    }
}