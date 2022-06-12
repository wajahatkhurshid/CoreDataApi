using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Gyldendal.Api.CoreData.Contracts.Enumerations
{
    /// <summary>
    /// These are different purchase options corresponding to OrderLineType in ShopServices.
    /// </summary>
    public enum PurchaseOptionType
    {
        [Display(Name = "Almindelig ordre")]
        Default = 0,

        //[Display(Name = "Forfatterkøb")]
        //AuthorCopies = 1,

        [Display(Name = "TeacherCopies")]
        [Description("Free copy for teachers.")]
        TeacherCopy = 2,

        [Display(Name = "InspectionCopies")]
        [Description("Free copy for inspection.")]
        InspectionCopy = 3,

        //[Display(Name = "Udleveret")]
        //InvoiceOnly = 4,

        //[Display(Name = "Særaftale")]
        //DigitalInvoiceOnly = 5,

        //[Display(Name = "Særaftale - genfakturering")]
        //DigitalInvoiceOnlyRebilling = 6,

        //[Display(Name = "Frieksemplar")]
        //FreeCopy = 7,

        //[Display(Name = "MANKO")]
        //Manko = 9
    }
}