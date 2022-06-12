using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public class AccessFormComparer : IEqualityComparer<AccessForm>
    {
        public bool Equals(AccessForm x, AccessForm y)
        {
            //Check whether the objects are the same object.
            if (ReferenceEquals(x, y)) return true;

            //Check whether the 'Period' properties are equal.
            return x != null && y != null && x.Code.Equals(y.Code) && x.PriceModels.All(pm => y.PriceModels.Any(ypm => ypm.Code == pm.Code));
        }

        public int GetHashCode(AccessForm obj)
        {
            //Get hash code for the Code field.
            var hashPeriodCode = obj.Code.GetHashCode();

            //Calculate the hash code for the product.
            return hashPeriodCode;
        }
    }
}