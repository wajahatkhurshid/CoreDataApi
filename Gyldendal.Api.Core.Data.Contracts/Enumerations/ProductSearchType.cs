using System.ComponentModel;

namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// The search type of Product, i.e. Phyiscal, Digital or Bundle
    /// </summary>
    public enum ProductSearchType
    {
        [Description("No product Search Type is specified")]
        None = 0,

        [Description("Product is of Physical type")]
        Physical = 1,

        [Description("Product is of Digital type")]
        Digital = 2,

        [Description("Product is of Bundle type")]
        Bundle = 3,
    }
}