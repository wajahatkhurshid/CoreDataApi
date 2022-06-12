using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.DataAccess.ShopAdmin;

namespace Gyldendal.Api.CoreData.ShopAdmin.Implementation
{
    public interface IPublizonProducts
    {
        List<PublizonMetadata> GetPublizonProductDetails(string isbn);
    }
}