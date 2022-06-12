using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class ProductResponse
    {
        public List<PressSiteProduct> Products { get; set; }
        public int Count { get; set; }
    }
}
