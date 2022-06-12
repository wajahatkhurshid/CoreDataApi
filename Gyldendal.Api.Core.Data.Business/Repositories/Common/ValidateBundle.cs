using Gyldendal.AccessServices.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public static class ValidateBundle
    {
        private static List<SalesConfiguration> _digitalSaleConfigs;

        /// <summary>
        /// Validate bundle product sale configurations.
        /// </summary>
        /// <param name="bundleProducts"></param>
        /// <returns></returns>
        public static bool ValidateBundleProducts(List<BundleProduct> bundleProducts)
        {
            var prodSaleConfigs = bundleProducts?.Select(x => new { Prod = x, SaleConfig = x.SalesConfiguration }).ToList();

            if (prodSaleConfigs?.Any(x => x.SaleConfig == null) ?? true)
            {
                return false;
            }

            if (prodSaleConfigs.Any(x => x.Prod.IsPhysical && !(x.SaleConfig.Approved)))
            {
                return false;
            }

            // if all producrs are physical
            if (prodSaleConfigs.All(x => x.Prod.IsPhysical))
            {
                return true;
            }

            _digitalSaleConfigs = prodSaleConfigs.Where(x => !(x.Prod.IsPhysical)).Select(x => x.SaleConfig).ToList();
            return ValidateDigitalBundleProducts();
        }

        /// <summary>
        /// Validating digital products saleconfigurations
        /// Rule # 1. SchoolLicense with heleSchoolen priceModel, teacherLicese with heleSchoolen priceModel and SingleUserLicense allowed in bundle.
        /// Rule # 2. Bundle is valid if all the bundleProduct have similar above mention allowed accessForms and similar billing periods.
        /// </summary>
        /// <returns></returns>
        private static bool ValidateDigitalBundleProducts()
        {
            var allowedSaleConfig = GetAllowedSaleConfiguration();

            // Remove not allowed accessForms from deflated saleConfiguration
            _digitalSaleConfigs.ForEach(x => x.AccessForms.RemoveAll(y => !allowedSaleConfig.Any(z => z.Key == y.Code)));

            // Remove not allowed priceModels from deflated saleConfiguration
            _digitalSaleConfigs.ForEach(x => x.AccessForms.ForEach(y => y.PriceModels.RemoveAll(z => !allowedSaleConfig[y.Code].Any(pm => pm.Code == z.Code))));

            // Remove accessForms other then singleUser which have no PriceModel after above step from deflated saleConfiguration
            _digitalSaleConfigs.ForEach(x => x.AccessForms.RemoveAll(y => y.Code != EnumAccessForm.SingleUser && (!y.PriceModels?.Any() ?? false)));

            return ValidateSaleForms() && ValidateAccessForms() && ValidateBillingPeriods();
        }

        /// <summary>
        /// Allowed SaleConfiguration on a bundle
        /// </summary>
        /// <returns></returns>
        private static Dictionary<EnumAccessForm, List<PriceModel>> GetAllowedSaleConfiguration()
        {
            return new Dictionary<EnumAccessForm, List<PriceModel>>
            {
                {
                    EnumAccessForm.School, new List<PriceModel> {
                    new PriceModel()
                    {
                        Code = Enums.EnumPriceModel.WholeSchool
                    }
                }},
                {
                    EnumAccessForm.Teacher,new List<PriceModel> {
                    new PriceModel()
                    {
                        Code = Enums.EnumPriceModel.WholeSchoolTeacher
                    }
                }},
                {
                    EnumAccessForm.SingleUser, null
                }
            };
        }

        /// <summary>
        /// Validate accessForm in all products of a bundle
        /// </summary>
        /// <returns></returns>
        private static bool ValidateSaleForms()
        {
            if ((_digitalSaleConfigs[0].SalesForms?.Any() ?? false) == false)
            {
                return false;
            }

            var flag = true;
            var saleFormComparer = new SaleFormComparer();

            for (var index = 0; index < _digitalSaleConfigs.Count && flag; index++)
            {
                var correspondingSaleConfig = _digitalSaleConfigs[index];
                var differenceAtoB = correspondingSaleConfig.SalesForms.Except(_digitalSaleConfigs[0].SalesForms, saleFormComparer);
                var differenceBtoA = _digitalSaleConfigs[0].SalesForms.Except(correspondingSaleConfig.SalesForms, saleFormComparer);

                if (differenceAtoB.Any() || differenceBtoA.Any())
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// Validate accessForm in all products of a bundle
        /// </summary>
        /// <returns></returns>
        private static bool ValidateAccessForms()
        {
            if ((_digitalSaleConfigs[0].AccessForms?.Any() ?? false) == false)
            {
                return false;
            }

            var flag = true;
            var accessFormComparer = new AccessFormComparer();

            for (var index = 0; index < _digitalSaleConfigs.Count && flag; index++)
            {
                var correspondingSaleConfig = _digitalSaleConfigs[index];
                var differenceAtoB = correspondingSaleConfig.AccessForms.Except(_digitalSaleConfigs[0].AccessForms, accessFormComparer);
                var differenceBtoA = _digitalSaleConfigs[0].AccessForms.Except(correspondingSaleConfig.AccessForms, accessFormComparer);

                if (differenceAtoB.Any() || differenceBtoA.Any())
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// Validate billing periods
        /// </summary>
        /// <returns></returns>
        private static bool ValidateBillingPeriods()
        {
            var accessForms = _digitalSaleConfigs[0].AccessForms.Select(x => x.Code).ToList();
            var flag = true;
            var billingComparer = new BillingPeriodComparer();

            for (var i = 0; i < accessForms.Count && flag; i++)
            {
                var correspondingBillingPeriods = _digitalSaleConfigs.Select(
                        x => x.AccessForms.FirstOrDefault(y => y.Code == accessForms[i])?.BillingPeriods).ToArray();

                if (correspondingBillingPeriods.Any() == false)
                {
                    return false;
                }

                for (var index = 1; index < correspondingBillingPeriods.Length; index++)
                {
                    var correspondingBillingPeriod = correspondingBillingPeriods[index];

                    var differenceAtoB = correspondingBillingPeriod.Except(correspondingBillingPeriods[0], billingComparer);
                    var differenceBtoA = correspondingBillingPeriods[0].Except(correspondingBillingPeriod, billingComparer);

                    if (differenceAtoB.Any() || differenceBtoA.Any())
                    {
                        flag = false;
                        break;
                    }
                }
            }

            return flag;
        }
    }
}