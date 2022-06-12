using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
   public interface IGGAProvider
    {
        SearchDtoResponse Search(string criteria, int mode);
    }
}
