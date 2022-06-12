using System;

namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// Enumeration to define scope of Data
    /// </summary>
    [Flags]
    public enum DataScope
    {
        Global = 0,
        BusinessShop = 1,
        GuShop = 2,
        HansReitzelShop = 4,
        MitForlagShop = 8,
        MunksGaardShop = 16,
        RosinantecoShop = 32,
        GyldendalDkShop = 64,
        EmployeeShop = 128,
        NotPublishedDigitalProducts = 256,
        GyldendalPlus = 512,
        StudyBox = 1024,
        TradeGyldendalDk = 2048,

        /// <summary>
        /// TEMPORARY - For verification of Front end integration of shadow implementation.
        /// Will be removed in future"
        /// </summary>
        TradeGyldendalDkShadow = 4096,

        /// <summary>
        /// TEMPORARY - For verification of Front end integration of shadow implementation.
        /// Will be removed in future"
        /// </summary>
        GyldendalPlusShadow = 8192,

        /// <summary>
        /// TEMPORARY - For verification of Front end integration of shadow implementation.
        /// Will be removed in future"
        /// </summary>
        GuShopShadow = 16384
    }
}