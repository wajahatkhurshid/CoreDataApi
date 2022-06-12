using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.DataAccess.ShopAdmin;
using Gyldendal.Api.CoreData.ShopAdmin.Implementation;

namespace Gyldendal.Api.CoreData.ShopAdmin.Infrastructure
{
    public class PublizonProducts : IPublizonProducts
    {
        private readonly ShopAdminEntities _shopAdminEntities;

        public PublizonProducts(ShopAdminEntities shopAdminEntities)
        {
            _shopAdminEntities = shopAdminEntities;
        }

        public List<PublizonMetadata> GetPublizonProductDetails(string isbn)
        {
            return _shopAdminEntities.PublizonMetadatas.Where(x => x.ISBN.Equals(isbn)).ToList();
            //.Select(x => new PublizonDto
            //{
            //    ISBN = x.ISBN,
            //    LastUpdated = x.LastUpdated,
            //    PublizonIdentifier = x.PublizonIdentifier,
            //    SampleURL = x.SampleURL
            //});
        }
    }
}