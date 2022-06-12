using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class SearchDtoResponse
    {
        public string TitleName { get; set; }

        public string ISBN { get; set; }

        public List<AuthorResponse> Authors { get; set; }

        public DateTime CreatedOn { get; set; }

        public int EditionNumber { get; set; }

        public string Translator { get; set; }

        public int Year { get; set; }

        public string Pages { get; set; }

        public string OriginalPublisher { get; set; }

        public string Illustrator { get; set; }

        public DateTime PublicationDate { get; set; }

    }
}
