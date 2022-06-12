using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public class SaleFormComparer : IEqualityComparer<SalesForm>
    {
        public bool Equals(SalesForm x, SalesForm y)
        {
            //Check whether the objects are the same object.
            if (ReferenceEquals(x, y)) return true;

            //Check whether the 'Period' properties are equal.
            return x != null && y != null && x.Code.Equals(y.Code);
        }

        public int GetHashCode(SalesForm obj)
        {
            //Get hash code for the Code field.
            var hashPeriodCode = obj.Code.GetHashCode();

            //Calculate the hash code for the product.
            return hashPeriodCode;
        }
    }
}