using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class Forfatter
    {
        public string Id { get; set; }
        public string ForfatterNavn { get; set; }
        public string Fornavn { get; set; }
        public string Efternavn { get; set; }
        public string ForfatterProfileLink { get; set; }
        public bool IsPhotoExists { get; set; }
        public Forfatterfoto Photo { get; set; }
    }
}
