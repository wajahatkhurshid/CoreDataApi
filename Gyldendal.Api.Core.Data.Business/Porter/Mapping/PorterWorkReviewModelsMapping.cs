using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Services.PorterApiClient;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;
using WorkReview = Gyldendal.Api.CoreData.Contracts.Models.WorkReview;

namespace Gyldendal.Api.CoreData.Business.Porter.Mapping
{
    public static class PorterWorkReviewModelsMapping
    {
        internal static WorkReview ToCoreDataWorkReview(this CoreData.Services.PorterApiClient.WorkReview workReview)
        {
            return new WorkReview
            {
                Id = workReview.Id,
                AuthorInfo = workReview.AuthorInfo,
                //  Draft = workReviewResponse.Draft,
                LastUpdated = workReview.UpdatedTimestamp,
                Rating = workReview.Rating,
                Review = workReview.Review,
                //  ReviewAttributeId = Convert.ToInt32(workReviewResponse.ReviewAttributeId),
                ShortDescription = string.Empty,
                WebShopId = WebShop.None,
                // TextType = workReviewResponse.TextType,
                //Title = workReviewResponse.,
                //Version = workReviewResponse.Version,
                WorkId = Convert.ToInt32(workReview.WorkId),
                WorkReviewId = Convert.ToInt32(workReview.Id)
            };
        }

        internal static List<WorkReviewUpdateInfo> ToCoreDataWorkReviewUpdateInfo(this ICollection<GetWorkReviewUpdateInfoResponse> workReviewUpdateInfoResponse)
        {
            return workReviewUpdateInfoResponse.Select(x => new WorkReviewUpdateInfo
            {
                WorkReviewId = Convert.ToInt32(x.WorkReviewId),
                UpdateTime = x.UpdateTime,
                UpdateType = x.UpdateType ? WorkReviewUpdateType.Deleted : WorkReviewUpdateType.Updated
            }).ToList();
        }
    }
}
