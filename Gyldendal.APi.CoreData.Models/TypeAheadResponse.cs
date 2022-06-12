using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class TypeAheadResponse
    {
        public List<Forfatterfoto> ListAuthors { get; set; }
        public List<BogForsider> ListBooks { get; set; }
        public List<BogForsider> ListKtitler { get; set; }
        public int CountKtitler { get; set; }
        public int CountAuthors { get; set; }
        public int CountBooks { get; set; }
    }
}
