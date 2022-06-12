using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.APi.CoreData.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IGgaService
    {
        Task<SearchDtoResponse> Search(string criteria, int mode);
    }
}
