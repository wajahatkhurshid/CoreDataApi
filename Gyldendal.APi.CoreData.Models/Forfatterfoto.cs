using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class Forfatterfoto
    {
        public string ImageSource { get; set; }
        public decimal? ImageSize { get; set; }
        public string Thumbnail { get; set; }
        public string Forfatter { get; set; }
        public string Kommentar { get; set; }
        public string Fotograf { get; set; }
        public string Ar { get; set; }
    }
}
