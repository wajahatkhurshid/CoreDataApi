using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class DeletedProduct
    {
        public string Isbn13 { get; set; }

        public DateTime? UpdateTime { get; set; }
    }
}
