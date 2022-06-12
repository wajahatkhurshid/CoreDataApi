using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.APi.CoreData.Models
{
    public class AuthorResponse
    {
        public string AuthorId { get; set; }

        public string AuthorName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AuthorUrlBig { get; set; }

        public string AuthorUrlMed { get; set; }

        public string AuthorUrlSmall { get; set; }
    }
}
