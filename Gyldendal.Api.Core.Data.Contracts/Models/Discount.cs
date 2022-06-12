namespace Gyldendal.Api.CoreData.Contracts.Models
{
    
    
    public class Discount
    {
        /// <summary>
        /// The id of the campaign the discount is associated with
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// /// The name of the campaign the discount is associated with
        /// </summary>
        public string CampaignName { get; set; }
        
        /// <summary>
        /// The actual percentage of the discount
        /// </summary>
        public string Percent { get; set; }

    }
}
