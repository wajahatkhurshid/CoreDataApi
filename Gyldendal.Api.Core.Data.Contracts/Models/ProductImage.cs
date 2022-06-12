namespace Gyldendal.Api.CoreData.Contracts.Models
{
    /// <summary>
    /// Product Image
    /// </summary>
    
    
    public class ProductImage
    {
        /// <summary>
        /// Url of the Images Gets or Sets
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Url { get; set; }
        /// <summary>
        /// Width of the Image Gets or Sets
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Height of the Image
        /// </summary>
        public int Height { get; set; }
    }
}
