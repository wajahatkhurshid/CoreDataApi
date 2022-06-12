using System;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.EventProcessor.Common;

namespace Gyldendal.Api.CoreData.EventProcessor.Contentful
{
    public class BaseContentfulEventHandler<T> : BaseEventHandler<T>
    {
        private readonly IKoncernDataUtils _koncernDataUtils;

        protected BaseContentfulEventHandler(IKoncernDataUtils koncernDataUtils, ILogger logger) : base(logger)
        {
            _koncernDataUtils = koncernDataUtils;
        }

        protected async Task RegisterContributorChangeFromThirdPartyAsync(DataScope dataScope, string source, string contributorId)
        {
            if (string.IsNullOrWhiteSpace(contributorId))
            {
                throw new ArgumentNullException(nameof(contributorId));
            }

            if (dataScope == DataScope.GyldendalDkShop || dataScope == DataScope.TradeGyldendalDk)
            {
                contributorId = contributorId.ToUpper().Replace("F", "0-");
            }

            await _koncernDataUtils.RegisterContributorChangeFromThirdPartyAsync(dataScope, source, contributorId);
        }
    }
}
