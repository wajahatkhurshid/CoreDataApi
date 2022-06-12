using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Util
{
    /// <summary>
    /// Contract for providing miscellaneous Koncerndata operations related to all Web Shops.
    /// </summary>
    public interface IKoncernDataUtils
    {
        bool IsShopDataAvailableForXMinutes(WebShop webShop, short xMinutes);
        
        IEnumerable<ProductBasicData> GetLicensedProductsByIsbn(WebShop webshop, IList<string> isbnList,bool getImageUrl=true);
        
        string GetSupplementaryData(string isbn);
        
        ProductAccessControlType GetProductAccessTypeByIsbn(string isbn);
        
        bool IsContributorDataAvailableForXMinutes(DataScope dataScope, short xMinutes);

        Task RegisterContributorChangeFromThirdPartyAsync(DataScope dataScope, string source, string contributorId);
    }
}
