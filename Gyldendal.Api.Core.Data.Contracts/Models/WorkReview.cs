using System;
using Gyldendal.Api.CommonContracts;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class WorkReview
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the attribute id of the review.
        /// </summary>
        /// <value>The review attribute id.</value>
        public int ReviewAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the review id of work.
        /// </summary>
        /// <value>The work review id.</value>
        public int WorkReviewId { get; set; }

        /// <summary>
        /// Gets or sets the work id.
        /// </summary>
        /// <value>The Work Id.</value>
        public int WorkId { get; set; }

        /// <summary>
        /// Gets or sets the Draft.
        /// </summary>
        /// <value>The draft text.</value>
        public bool? Draft { get; set; }

        /// <summary>
        /// Gets or sets the LastUpdated date & time.
        /// </summary>
        /// <value>Last Updated Time.</value>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        /// <value>The Rating value (if any).</value>
        public int? Rating { get; set; }

        /// <summary>
        /// Gets or sets the review.
        /// </summary>
        /// <value>The Review text.</value>
        public string Review { get; set; }

        /// <summary>
        /// Gets or sets short description.
        /// </summary>
        /// <value>The short description.</value>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title string.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the author description/info.
        /// </summary>
        /// <value>The about author text.</value>
        public string AuthorInfo { get; set; }

        /// <summary>
        /// Gets or sets the type of text (e.g. review).
        /// </summary>
        /// <value>The text type (e.g. review).</value>
        public string TextType { get; set; }

        /// <summary>
        /// Gets or sets the Version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets website id.
        /// </summary>
        public WebShop WebShopId { get; set; }

        /// <summary>
        /// Gets or sets Priority of review.
        /// </summary>
        public int Priority { get; set; }
    }
}