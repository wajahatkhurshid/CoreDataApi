using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gyldendal.Api.CoreData.Common;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public class ErrorCodeUtil : IErrorCodeUtil
    {
        /// <summary>
        /// Get All ErrorCodes Information
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ErrorDetail> GetAllErrorCodes()
        {
            var errorsInfo = new List<ErrorDetail>();
            errorsInfo.AddRange(Extensions.GetValues<ErrorCodes>().Select(x => x.GetErrorDetail()));
            return errorsInfo;
        }

        /// <summary>
        /// Get ErrorCode Details 
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public ErrorDetail GetErrorDetail(ulong errorCode)
        {
            try
            {
                var enumVal = Extensions.GetEnum<ErrorCodes>(errorCode);

                return GetErrorDetails(enumVal);
            }
            catch (InvalidEnumArgumentException)
            {
                // Supress exception error code not found
            }

            return GetErrorDetails(ErrorCodes.InvalidErrorCode);
        }

        private ErrorDetail GetErrorDetails(ErrorCodes errorCode)
        {
            return new ErrorDetail
            {
                Code = (ulong) errorCode,
                Description = errorCode.GetDescription(),
                OriginatingSystem = Extensions.CoreDataSystemName
            };
        }
    }
}
