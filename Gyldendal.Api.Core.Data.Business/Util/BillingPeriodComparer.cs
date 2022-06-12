using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public class BillingPeriodComparer : IEqualityComparer<Period>
    {
        public bool Equals(Period x, Period y)
        {
            //Check whether the objects are the same object.
            if (ReferenceEquals(x, y)) return true;

            //Check whether the 'Period' properties are equal.
            return x != null && y != null && x.Code.Equals(y.Code) && x.RefPeriodUnitTypeCode.Equals(y.RefPeriodUnitTypeCode) && x.UnitValue.Equals(y.UnitValue);
        }

        public int GetHashCode(Period obj)
        {
            //Get hash code for the RefPeriodUnitTypeCode field.
            var hashPeriodUnitType = obj.RefPeriodUnitTypeCode.GetHashCode();

            //Get hash code for the Code field.
            var hashPeriodCode = obj.Code.GetHashCode();

            //Get hash code for the UnitValue field.
            var hashUnitValue = obj.UnitValue.GetHashCode();

            //Calculate the hash code for the product.
            return hashPeriodUnitType ^ hashPeriodCode ^ hashUnitValue;
        }
    }
}