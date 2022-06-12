using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class BogForsider
    {
        public string ImageSource { get; set; }
        public string Titel { get; set; }
        public string Forfatters { get; set; }
        public string BookDetailLink { get; set; }
        public string OstISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Organization { get; set; }
        public DateTime PublishDate { get; set; }

    }
}
