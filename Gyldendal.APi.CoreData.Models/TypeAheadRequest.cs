using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
   public class TypeAheadRequest
    {
        public string PrefixText { get; set; }
        public DateTime FromDateK { get; set; }
        public DateTime ToDateK { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
