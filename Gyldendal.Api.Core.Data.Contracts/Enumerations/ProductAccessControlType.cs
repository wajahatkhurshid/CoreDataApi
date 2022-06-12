using System.ComponentModel;

namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// Type of Product adgangskontrol i.e. None, Ekey or Unice
    /// </summary>
    public enum ProductAccessControlType
    {
        [Description("No product access control type is specified")]
        None = 0,

        [Description("Product is of ekey")]
        Ekey = 1,

        [Description("Product is of unic/institution")]
        Unic = 2
    }
}