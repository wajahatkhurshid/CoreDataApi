using System;
using SolrNet.Attributes;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.SolrContracts.ClubPeriod
{
	public class ClubPeriod
	{
		[SolrUniqueKey("id")]
		public int Id { get; set; }
	 
		[SolrField("periodCode")]
		public string PeriodCode { get; set; }
	 
		[SolrField("periodName")]
		public string PeriodName { get; set; }
	 
		[SolrField("cancellationDeadline")]
		public DateTime CancellationDeadline { get; set; }
	 
		[SolrField("startDate")]
		public DateTime StartDate { get; set; }
	 
		[SolrField("endDate")]
		public DateTime EndDate { get; set; }
	 
		[SolrField("clubCode")]
		public string ClubCode { get; set; }
	 
		[SolrField("clubName")]
		public string ClubName { get; set; }
	 
		[SolrField("branchCode")]
		public string BranchCode { get; set; }
	 
		[SolrField("branchName")]
		public string BranchName { get; set; }
	 
		[SolrField("productId")]
		public string ProductId { get; set; }
	 
		[SolrField("lastUpdated")]
		public DateTime LastUpdated { get; set; }
	}

	public enum ClubPeriodSchemaField
	{
		[ClubPeriodSchemaFieldMeta("id")]
		Id,
		[ClubPeriodSchemaFieldMeta("periodCode")]
		PeriodCode,
		[ClubPeriodSchemaFieldMeta("periodName")]
		PeriodName,
		[ClubPeriodSchemaFieldMeta("cancellationDeadline")]
		CancellationDeadline,
		[ClubPeriodSchemaFieldMeta("startDate")]
		StartDate,
		[ClubPeriodSchemaFieldMeta("endDate")]
		EndDate,
		[ClubPeriodSchemaFieldMeta("clubCode")]
		ClubCode,
		[ClubPeriodSchemaFieldMeta("clubName")]
		ClubName,
		[ClubPeriodSchemaFieldMeta("branchCode")]
		BranchCode,
		[ClubPeriodSchemaFieldMeta("branchName")]
		BranchName,
		[ClubPeriodSchemaFieldMeta("productId")]
		ProductId,
		[ClubPeriodSchemaFieldMeta("lastUpdated")]
		LastUpdated,

	}
}
