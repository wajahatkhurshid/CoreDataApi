using System;
using SolrNet.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.SolrContracts.Contributor
{
	public class Contributor
	{
		[SolrUniqueKey("id")]
		public string Id { get; set; }
	 
		[SolrField("contributorid")]
		public string ContributorId { get; set; }
	 
		[SolrField("description")]
		public string Description { get; set; }
	 
		[SolrField("contributorname")]
		public string ContributorName { get; set; }
	 
		[SolrField("imagesJson")]
		public string ImagesJson { get; set; }
	 
		[SolrField("websiteId")]
		public int WebsiteId { get; set; }
	 
		[SolrField("firstname")]
		public string FirstName { get; set; }
	 
		[SolrField("lastname")]
		public string LastName { get; set; }
	 
		[SolrField("searchname")]
		public string SearchName { get; set; }
	 
		[SolrField("lastUpdated")]
		public DateTime LastUpdated { get; set; }
	 
		[SolrField("exactmatchfield")]
		public string[] ExactMatch { get; set; }
	 
		[SolrField("substringfield")]
		public string[] SubstringField { get; set; }
	}

	public enum ContributorSchemaField
	{
		[ContributorSchemaFieldMeta("id")]
		Id,
		[ContributorSchemaFieldMeta("contributorid")]
		ContributorId,
		[ContributorSchemaFieldMeta("description")]
		Description,
		[ContributorSchemaFieldMeta("contributorname")]
		ContributorName,
		[ContributorSchemaFieldMeta("imagesJson")]
		ImagesJson,
		[ContributorSchemaFieldMeta("websiteId")]
		WebsiteId,
		[ContributorSchemaFieldMeta("firstname")]
		FirstName,
		[ContributorSchemaFieldMeta("lastname")]
		LastName,
		[ContributorSchemaFieldMeta("searchname")]
		SearchName,
		[ContributorSchemaFieldMeta("lastUpdated")]
		LastUpdated,
		[ContributorSchemaFieldMeta("exactmatchfield")]
		ExactMatch,
		[ContributorSchemaFieldMeta("substringfield")]
		SubstringField,
		[ContributorSchemaFieldMeta("contributornameSorting")]
		ContributorNameSorting,
		[ContributorSchemaFieldMeta("firstnameSorting")]
		FirstNameSorting,
		[ContributorSchemaFieldMeta("lastnameSorting")]
		LastNameSorting,

	}
}
