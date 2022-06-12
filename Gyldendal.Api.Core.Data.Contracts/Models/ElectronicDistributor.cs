using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    
    
    public class ElectronicDistributor
    {
        public int Id { get; set; }
        public string SampleUrl { get; set; }
        public ElectronicDistributorType DistributorName { get; set; }
    }
}
