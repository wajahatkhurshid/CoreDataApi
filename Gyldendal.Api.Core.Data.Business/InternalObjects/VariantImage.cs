using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.InternalObjects
{
    public class VariantImage
    {
        public List<ProductImage> ProductImages { get; set; }
        public string OriginalCoverImageUrl { get; set; }
    }
}
