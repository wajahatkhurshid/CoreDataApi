using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public static class Constants
    {
        public const string DeleteAction = "Deleted";

        public const decimal VatPercentage = 25;

        /// <summary>
        /// Temporary list of isbns of HR and Munks for which teacher sample copy is not allowed.
        /// todo: Enhance this process to skip isbns for some purchase option in more manageable way.
        /// </summary>
        public static List<string> HrmIsbnWithoutTeacherSamplePurchaseOption = new List<string>
        {
            "9788762817524",
            "9788762818378",
            "9788799323647",
            "9788741257532",
            "9788741257587",
            "9788741273082",
            "9788799323630",
            "9788741257488",
            "9788741266596",
            "9788741273099",
            "9788741278414",
            "9788776758493",
            "9788799323692",
            "9788741260983",
            "9788799323623",
            "9788799323685",
            "9788799796229",
            "9788741256344",
            "9788741273273",
        };
    }
}