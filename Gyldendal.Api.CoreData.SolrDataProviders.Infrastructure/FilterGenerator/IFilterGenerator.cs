using System.Collections.Generic;
using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.Models;

// ReSharper disable IdentifierTypo

namespace Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure.FilterGenerator
{
    public interface IFilterGenerator<in T> where T : FilterGenerationInput
    {
        IEnumerable<FilterInfo> Generate(T input);
    }
}