using System;
using SolrNet.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.SolrContracts.WorkReview
{
	public class WorkReview
	{
		[SolrUniqueKey("id")]
		public string Id { get; set; }
	 
		[SolrField("workReviewId")]
		public int WorkReviewId { get; set; }
	 
		[SolrField("workId")]
		public int WorkId { get; set; }
	 
		[SolrField("websiteId")]
		public int WebsiteId { get; set; }
	 
		[SolrField("reviewAttributeId")]
		public int ReviewAttributeId { get; set; }
	 
		[SolrField("draft")]
		public bool Draft { get; set; }
	 
		[SolrField("lastupdated")]
		public DateTime LastUpdated { get; set; }
	 
		[SolrField("rating")]
		public int Rating { get; set; }
	 
		[SolrField("review")]
		public string Review { get; set; }
	 
		[SolrField("shortDescription")]
		public string ShortDescription { get; set; }
	 
		[SolrField("titleda")]
		public string Titleda { get; set; }
	 
		[SolrField("aboutAuthor")]
		public string AboutAuthor { get; set; }
	 
		[SolrField("textType")]
		public string TextType { get; set; }
	 
		[SolrField("priority")]
		public int Priority { get; set; }
	}

	public enum WorkReviewSchemaField
	{
		[WorkReviewSchemaFieldMeta("id")]
		Id,
		[WorkReviewSchemaFieldMeta("workReviewId")]
		WorkReviewId,
		[WorkReviewSchemaFieldMeta("workId")]
		WorkId,
		[WorkReviewSchemaFieldMeta("websiteId")]
		WebsiteId,
		[WorkReviewSchemaFieldMeta("reviewAttributeId")]
		ReviewAttributeId,
		[WorkReviewSchemaFieldMeta("draft")]
		Draft,
		[WorkReviewSchemaFieldMeta("lastupdated")]
		LastUpdated,
		[WorkReviewSchemaFieldMeta("rating")]
		Rating,
		[WorkReviewSchemaFieldMeta("review")]
		Review,
		[WorkReviewSchemaFieldMeta("shortDescription")]
		ShortDescription,
		[WorkReviewSchemaFieldMeta("titleda")]
		Titleda,
		[WorkReviewSchemaFieldMeta("aboutAuthor")]
		AboutAuthor,
		[WorkReviewSchemaFieldMeta("textType")]
		TextType,
		[WorkReviewSchemaFieldMeta("priority")]
		Priority,

	}
}
