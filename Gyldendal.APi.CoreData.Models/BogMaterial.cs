using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class BogMaterial
    {
        public BogMaterialType MaterialType { get; set; }
        public String MaterialURI { get; set; }
        public string MaterialTitel { get; set; }
    }
}
