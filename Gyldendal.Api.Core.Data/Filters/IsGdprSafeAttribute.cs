using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gyldendal.Api.CoreData.Filters
{
    /// <summary>
    /// This attribute will be used to set the isGdprSafe bit in logging database for all the incoming requests.
    /// Whenever this attribute is used on any action method or controller, it will determine that the data in the incoming request is GDPR safe or not.
    /// </summary>
    public class IsGdprSafeAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        public bool IsGdprSafe;
        /// <summary>
        /// </summary>
        /// <param name="isGdprSafe"></param>
        public IsGdprSafeAttribute(bool isGdprSafe)
        {
            IsGdprSafe = isGdprSafe;
        }
    }
}