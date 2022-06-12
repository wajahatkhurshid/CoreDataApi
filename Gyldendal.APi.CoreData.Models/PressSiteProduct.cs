using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    /// <summary>
    /// Added prefix of presssite because while generating clinet in press site model name was changed to product2 becasue product class already exists in CoreDataAPi
    /// </summary>
    public class PressSiteProduct
    {
        public string VareId { get; set; }
        public string Titel { get; set; }
        public string Subtitle { get; set; }
        public DateTime? PublishDate { get; set; }
        public string PublishedYear { get; set; }
        public string MediaType { get; set; }
        public string ImageSource { get; set; }
        public int ImageSize { get; set; }
        public string Authors { get; set; }
        public IQueryable<string> AuthorsList { get; set; }
        public string Isbn10 { get; set; }
        public string Isbn13 { get; set; }
        public int? Edition { get; set; }
        public string Genudg { get; set; }
        public string NumberOfPages { get; set; }
        public string Description { get; set; }
        public decimal? RetailPrice { get; set; }
        public string Omslagsgrafiker { get; set; }
        public string Oversatter { get; set; }
        public string Organization { get; set; }
        public string Category { get; set; }
        public int? CategoryOrder { get; set; }
        public string TopAuthor { get; set; }

        public string Kontaktperson { get; set; }
        public string KontaktpersonFone { get; set; }
        public string kontactperosnEmail { get; set; }

        public List<Forfatter> Forfattere { get; set; }
        public List<BogMaterial> SiteCoreMaterial { get; set; }

        // Below Fields added as per S 1710-3208 by HJ 26-10-2017

        public int WorkId { get; set; }

        public string WorkTitle { get; set; }
    }
}
