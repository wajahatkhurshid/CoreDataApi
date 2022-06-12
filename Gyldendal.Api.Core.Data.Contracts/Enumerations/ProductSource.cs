using System.ComponentModel;
namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// The product source is the orignal source (Webshop) from which 
    /// a product is comming in Solr. For Example in Gyldendal.dk we have GU products as well.
    /// For these prodcuts in Solr product source will be Gu.
    /// </summary>
    public enum ProductSource
    {
        [Description("No Source")]
        None = 0,

        [Description("RAP")]
        Rap = 1,

        [Description("Gyldendal.Dk")]
        Gyldendal = 2,

        [Description("Gyldendal Uddannelse")]
        Gu = 3,

        [Description("StudyBox")]
        StudyBox = 4
    }
}
