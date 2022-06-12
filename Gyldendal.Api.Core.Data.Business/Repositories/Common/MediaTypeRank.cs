using Gyldendal.Api.CoreData.Common;
using System.ComponentModel;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public class MediaTypeRank
    {
        public static int GetMediaTypeRank(string mediaType)
        {
            return (int)mediaType.GetValueFromDescription<EnumMediaTypeRank>();
        }

        private enum EnumMediaTypeRank
        {
            None = 0,

            [Description("bog")]
            Bog = 1000,

            [Description("i-bog")]
            IBog = 990,

            [Description("website")]
            website = 980,

            [Description("online")]
            Online = 970,

            [Description("internet")]
            Internet = 960,

            [Description("e-bog (pdf-format)")]
            EbogPDF = 950,

            [Description("e-bog")]
            Ebog = 940,

            [Description("e-bog (epub2)")]
            EbogEPUB2 = 930,

            [Description("e-bog (epub3)")]
            EbogEPUB3 = 920,

            [Description("e-bog (epub3fxl)")]
            EbogEPUB3FXL = 910,

            [Description("cd")]
            CD = 900,

            [Description("dvd")]
            DVD = 890,

            [Description("lydfiler")]
            Lydfiler = 880,

            [Description("iphone applikation")]
            IphoneApp = 870,

            [Description("app (iphone/ipad)")]
            AppIphoneIpad = 860,

            [Description("plakater")]
            Plakater = 850,

            [Description("plakat")]
            Plakat = 840,

            [Description("brochure")]
            Brochure = 830,

            [Description("marketing mat")]
            MarketingMat = 820,

            [Description("andet")]
            Andet = 810,

            [Description("package")]
            Package = 800,

            
        }
    }
}