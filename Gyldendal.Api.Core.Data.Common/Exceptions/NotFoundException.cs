using System;

namespace Gyldendal.Api.CoreData.Common.Exceptions
{
    public class NotFoundException: Exception
    {
        public override string Message { get; }
        public NotFoundException(string message)
        {
            Message = message;
        }
    }
}
