using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.ShopServices.ApiClient;
using Gyldendal.Api.ShopServices.Contracts.Discount;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using NewRelic.Api.Agent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.ExtensionMethods
{
    public static class Extension
    {
        /// <summary>
        /// This method will generate sales configuration based on provided price.
        /// for example: Assigning Physical Product Sale Configuration.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="unitPriceWithVat"></param>
        /// <param name="unitPriceWithoutVat"></param>
        /// <returns></returns>
        public static void SetProductSaleConfigurationOnFly(this Product product, decimal unitPriceWithVat, decimal unitPriceWithoutVat)
        {
            var salesConfiguration = new SalesConfiguration
            {
                Isbn = product.Isbn13,
                SalesForms = new List<SalesForm> { new SalesForm { Code = EnumLicenseType.None } },
                Approved = true,
                CreatedDate = product.LastUpdated,
                AccessForms = new List<AccessForm>
                {
                    new AccessForm
                    {
                        Code = product.IsPhysical ? EnumAccessForm.None : EnumAccessForm.SingleUser,
                        BillingPeriods = new List<Period>
                        {
                            new Period
                            {
                                UnitValue = 0,
                                Code = Enums.EnumPeriodType.None,
                                Price = new Price
                                {
                                    UnitPriceVat = unitPriceWithVat,
                                    UnitPrice = unitPriceWithoutVat
                                }
                            }
                        }
                    }
                }
            };
            product.SalesConfiguration = salesConfiguration;
        }

        /// <summary>
        /// for Assigning Physical Product Sale Configuration to a bundle product
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="unitPriceWithVat"></param>
        /// <param name="unitPriceWithoutVat"></param>
        /// <param name="lastUpdated"></param>
        /// <param name="salesConfiguration"></param>
        /// <returns></returns>
        public static SalesConfiguration SetPhysicalBundleProductSaleConfiguration(this SalesConfiguration salesConfiguration, string isbn, decimal unitPriceWithVat,
            decimal unitPriceWithoutVat, DateTime lastUpdated)
        {
            salesConfiguration.Isbn = isbn;
            salesConfiguration.SalesForms = new List<SalesForm> { new SalesForm { Code = EnumLicenseType.None } };
            salesConfiguration.Approved = true;
            salesConfiguration.CreatedDate = lastUpdated;
            salesConfiguration.AccessForms = new List<AccessForm>
            {
                new AccessForm
                {
                    Code = EnumAccessForm.None,
                    BillingPeriods = new List<Period>
                    {
                        new Period
                        {
                            UnitValue = 0,
                            Code = Enums.EnumPeriodType.None,
                            RefPeriodUnitTypeCode = EnumPeriodUnitType.None,
                            Price = new Price
                            {
                                UnitPriceVat = unitPriceWithVat,
                                UnitPrice = unitPriceWithoutVat
                            }
                        }
                    }
                }
            };

            return salesConfiguration;
        }

        /// <summary>
        /// Calculate the price of bundle and discounted price of bundle's product
        /// call after validating sale configuration of bundle products
        /// </summary>
        /// <param name="bundleProductSaleConfig"></param>
        /// <param name="discountPercentage"></param>
        /// <returns></returns>
        public static List<BundleProductPrice> CalculateBundlePrice(this SalesConfiguration bundleProductSaleConfig, decimal discountPercentage)
        {
            if (bundleProductSaleConfig?.AccessForms == null || !bundleProductSaleConfig.AccessForms.Any()
                || bundleProductSaleConfig.AccessForms.All(x => x.BillingPeriods == null))
            {
                return null;
            }

            var productPrices = new List<BundleProductPrice>();
            bundleProductSaleConfig.AccessForms.ForEach(x => x.BillingPeriods.ForEach(y =>
            {
                productPrices.Add(new BundleProductPrice
                {
                    DiscountPercentage = discountPercentage,
                    Price = y.Price.UnitPrice,
                    PeriodDisplayName = y.DisplayName,
                    PriceWithVat = y.Price.UnitPriceVat,
                    DiscountedPrice = y.Price.UnitPrice.CalculateDiscount(discountPercentage),
                    DiscountedPriceWithVat = y.Price.UnitPriceVat.CalculateDiscount(discountPercentage),
                    PeriodTypeCode = y.Code,
                    PeriodUnitTypeCode = y.RefPeriodUnitTypeCode,
                    PeriodUnitValue = y.UnitValue,
                    AccessFormCode = x.Code,
                });
            }));

            return productPrices;
        }

        /// <summary>
        /// return number, roundoff with nearest 0.5 to floor
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static decimal RoundToNearestHalf(this decimal number)
        {
            return Math.Floor(number * 2) / 2;
        }

        /// <summary>
        /// Return discount on base of given price and discount percentage
        /// </summary>
        /// <param name="price"></param>
        /// <param name="discountPercentage"></param>
        /// <returns></returns>
        public static decimal CalculateDiscount(this decimal price, decimal discountPercentage)
        {
            return (price - (price * (discountPercentage / 100)));
        }

        #region [ Wbshop Mapping to KD Shop Name ]

        /// <summary>
        /// Gyldendal API site to Ekey's webshop name
        /// </summary>
        private static readonly Dictionary<WebShop, string> WebShopName = new Dictionary<WebShop, string>
        {
            { WebShop.None , null},
            {WebShop.Business , "BUSINESS"},
            {WebShop.Gu , "GU"},
            {WebShop.HansReitzel , "Hans Reitzel"},
            {WebShop.MitForlag , "MitForlag"},
            {WebShop.MunksGaard , "Munksgaard"},
            {WebShop.Rap , "RAP"},
            {WebShop.Rosinanteco , "ROSINANTECO"},
            {WebShop.GyldendalDk, "GDK" }
        };

        /// <summary>
        /// Extension method to get Koncerndata webshop name against the webshop seller enum
        /// </summary>
        /// <param name="webShop">Enum value of API webshop</param>
        /// <returns>string value containing seller name koncerndata_webshop</returns>
        public static string KdWebshopName(this WebShop webShop)
        {
            return WebShopName.ContainsKey(webShop) ? WebShopName[webShop] : null;
        }

        /// <summary>
        /// Extension method to get webshop seller enum from Koncerndata webshop name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebShop WebshopEnum(this string value)
        {
            return WebShopName.ContainsValue(value) ? WebShopName.First(x => x.Value == value).Key : WebShop.None;
        }

        #endregion [ Wbshop Mapping to KD Shop Name ]

        #region [ DataScope Mapping to Wbshop ]

        /// <summary>
        /// Webshop Enumeration mapping to DataScope Enumeration
        /// </summary>
        private static readonly Dictionary<WebShop, DataScope> DataScopeWebShopMappings =
            new Dictionary<WebShop, DataScope>
            {
                {WebShop.None, DataScope.NotPublishedDigitalProducts},
                {WebShop.Business, DataScope.BusinessShop},
                {WebShop.Gu, DataScope.GuShop},
                {WebShop.HansReitzel, DataScope.HansReitzelShop},
                {WebShop.MitForlag, DataScope.MitForlagShop},
                {WebShop.MunksGaard, DataScope.MunksGaardShop},
                {WebShop.GyldendalDk, DataScope.GyldendalDkShop},
                {WebShop.EmployeeShop, DataScope.EmployeeShop},
                {WebShop.Rosinanteco, DataScope.RosinantecoShop},

                {WebShop.ClubBogklub, DataScope.GyldendalPlus},
                {WebShop.ClubBoerne, DataScope.GyldendalPlus},
                {WebShop.ClubSamleren, DataScope.GyldendalPlus},
                {WebShop.ClubKrimi, DataScope.GyldendalPlus},
                {WebShop.ClubPsykeSjael, DataScope.GyldendalPlus},
                {WebShop.ClubHistorie, DataScope.GyldendalPlus},
                {WebShop.ClubPaedagogisk, DataScope.GyldendalPlus},
                {WebShop.ClubBoerne3To5, DataScope.GyldendalPlus},
                {WebShop.ClubBoerne5To10, DataScope.GyldendalPlus},
                {WebShop.ClubFlamingo, DataScope.GyldendalPlus},
                {WebShop.ClubSundtLiv, DataScope.GyldendalPlus},
                {WebShop.TradeGyldendalDk, DataScope.TradeGyldendalDk},
                {WebShop.StudyBox, DataScope.StudyBox}
            };

        /// <summary>
        /// Extension method to get WebShop Enum against DataScope Enum
        /// </summary>
        /// <param name="webShop">Enum value of webShop</param>
        /// <returns>Enumeration value of WebShop</returns>
        [Trace]
        public static DataScope ToDataScope(this WebShop webShop)
        {
            if (DataScopeWebShopMappings.ContainsKey(webShop))
            {
                return DataScopeWebShopMappings[webShop];
            }

            throw new InvalidOperationException("Unable to map web shop");
        }

        public static WebShop ToWebShop(this DataScope dataScope)
        {
            if (DataScopeWebShopMappings.ContainsValue(dataScope))
            {
                return (from entry in DataScopeWebShopMappings
                        where entry.Value == dataScope
                        select entry.Key).SingleOrDefault();
            }

            throw new InvalidOperationException("Unable to map data scope");
        }

        public static WebShop ToFirstWebShop(this DataScope dataScope)
        {
            if (DataScopeWebShopMappings.ContainsValue(dataScope))
            {
                return (from entry in DataScopeWebShopMappings
                        where entry.Value == dataScope
                        select entry.Key).FirstOrDefault();
            }

            throw new InvalidOperationException("Unable to map data scope");
        }

        public static List<WebShop> ToWebShops(this DataScope dataScope)
        {
            if (dataScope == DataScope.Global)
            {
                return DataScopeWebShopMappings.Select(a => a.Key).ToList();
            }
            return DataScopeWebShopMappings.Where(a => (a.Value & (dataScope)) == a.Value).Select(a => a.Key).ToList(); //fetch webshops using bitwise operation...

            throw new InvalidOperationException("Unable to map data scope");
        }

        #endregion [ DataScope Mapping to Wbshop ]

        /// <summary>
        /// This method will generate PriceInfo with defaultPrice based on provided prices.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="unitPriceWithVat"></param>
        /// <param name="unitPriceWithoutVat"></param>
        /// <param name="discountProductPrice"></param>
        /// <returns></returns>
        public static void SetProductDefaultPriceInfo(this Product product, decimal unitPriceWithVat, decimal unitPriceWithoutVat,
            DiscountProductPrice discountProductPrice = null)
        {
            var productInfo = new PricingInfo()
            {
                DefaultPrice = new ProductPrice()
                {
                    PriceWithVat = unitPriceWithVat,
                    PriceWithoutVat = unitPriceWithoutVat,
                    ValidTo = DateTime.MaxValue,
                    ValidFrom = DateTime.MinValue,
                    PriceType = ProductPriceType.Default,
                    VatPercentage = Repositories.Common.Constants.VatPercentage,
                }
            };

            if (discountProductPrice != null)
            {
                productInfo.DefaultPrice.DiscountedPriceWithVat = discountProductPrice.PriceIncVatAndDiscount;
                productInfo.DefaultPrice.DiscountedPriceWithoutVat = discountProductPrice.PriceIncDiscount;
                productInfo.DefaultPrice.DiscountPercentage = discountProductPrice.DiscountPercentage;
            }

            product.PricingInfo = productInfo;
        }

        public static void SetProductSalesConfiguration(this Product product, decimal unitPriceWithoutTax, decimal unitPriceWithTax,
            Client shopServicesApiClient)
        {
            if (product.IsPhysical)
            {
                product.SetProductSaleConfigurationOnFly(unitPriceWithTax, unitPriceWithoutTax);
            }
            else
            {
                product.SalesConfiguration =
                    shopServicesApiClient.SalesConfiguration.GetSalesConfiguration(product.Isbn13, product.WebShop);
            }
        }
    }
}