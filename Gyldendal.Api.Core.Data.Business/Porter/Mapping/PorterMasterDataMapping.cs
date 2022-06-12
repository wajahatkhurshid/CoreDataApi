using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Mapping
{
    public static class PorterMasterDataMapping
    {
        public static Level ToCoreDataLevel(this CoreData.Services.PorterApiClient.Level porterLevel, WebShop webshop)
        {
            return new Level
            {
                AreaId = porterLevel.AreaId,
                LevelNumber = porterLevel.LevelNumber,
                Name = porterLevel.Name,
                WebShop = webshop
            };
        }

        public static SubArea ToCoreDataSubArea(this CoreData.Services.PorterApiClient.SubArea subArea, WebShop webshop)
        {
            return new SubArea
            {
                Id = subArea.Id,
                Name = subArea.Name,
                SubjectId = subArea.SubjectId,
                WebShop = webshop
            };
        }

        public static Subject ToCoreDataSubjects(this CoreData.Services.PorterApiClient.Subject subject, WebShop webshop)
        {
            return new Subject
            {
                Id = subject.Id,
                Name = subject.Name,
                AreaId = subject.AreaId,
                WebShop = webshop
            };
        }

        public static ThemaCode ToCoreDataThemaCode(this CoreData.Services.PorterApiClient.SubjectCode subjectCode)
        {
            return new ThemaCode()
            {
                Id = subjectCode.Id,
                Code = subjectCode.Code,
                Description = subjectCode.Code //todo
            };
        }
    }
}
