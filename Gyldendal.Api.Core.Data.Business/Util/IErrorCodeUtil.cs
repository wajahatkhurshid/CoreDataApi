using System.Collections.Generic;
using Gyldendal.Common.WebUtils.Models;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public interface IErrorCodeUtil
    {
        IEnumerable<ErrorDetail> GetAllErrorCodes();

        ErrorDetail GetErrorDetail(ulong errorCode);
    }
}
