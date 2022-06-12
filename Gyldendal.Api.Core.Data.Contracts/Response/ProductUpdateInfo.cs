using Gyldendal.Api.CoreData.Contracts.Enumerations;
using System;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class ProductUpdateInfo
    {
        public string ProductId { get; set; }

        public ProductUpdateType UpdateType { get; set; }

        public ProductType ProductType { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}