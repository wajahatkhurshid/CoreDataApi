using System.ComponentModel;

namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// Type of bundle i.e. Single or bundle
    /// </summary>
    public enum ProductType
    {
        [Description("No product Type is specified")]
        None = 0,

        [Description("Single Product")]
        SingleProduct = 1,

        [Description("Product is of Bundle type")]
        Bundle = 2,
    }
}