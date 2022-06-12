using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
   public class AuthorsRequest
    {
        public string SearchString { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortExpresion { get; set; }
    }
}
