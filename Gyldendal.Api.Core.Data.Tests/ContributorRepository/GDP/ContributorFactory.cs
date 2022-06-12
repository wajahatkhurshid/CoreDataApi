using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Moq;
using System;

namespace Gyldendal.Api.CoreData.Tests.ContributorRepository.GDP
{
    public static class ContributorFactory
    {
        public static Mock<koncerndata_webshops_Entities> KdEntities;

        public static void Setup()
        {
            KdEntities = new Mock<koncerndata_webshops_Entities>(MockBehavior.Loose);
            var productContributorsLogDbSet = new FakeDbSet<DEA_KDWS_GPlusproductcontributorsLog>
            {
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-11745", vare_id = "9788702176629", role_id = 16, Action = "Updated"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-12994", vare_id = "9788702242768", role_id = 7,  Action = "New"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-38145", vare_id = "9788777493867", role_id = 2,  Action = "Deleted"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-79876", vare_id = "9788702164916", role_id = 7,  Action = "New"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-81742", vare_id = "9788702059762", role_id = 7,  Action = "New"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-11749", vare_id = "9788702176629", role_id = 16, Action = "Updated"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-12951", vare_id = "9788702242768", role_id = 7,  Action = "New"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-38153", vare_id = "9788777493867", role_id = 2,  Action = "Deleted"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-79855", vare_id = "9788702164916", role_id = 7,  Action = "New"},
                new DEA_KDWS_GPlusproductcontributorsLog {contributor_id = "0-81757", vare_id = "9788702059762", role_id = 7,  Action = "New"},
            };
            var contributorsLogsDbSet = new FakeDbSet<DEA_KDWS_GPlusContributorsLog>
            {
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-11745", CreatedDate = new DateTime(2020, 08, 12, 03, 39, 27), Action = "New"    },
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-12994", CreatedDate = new DateTime(2020, 08, 11, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-38145", CreatedDate = new DateTime(2020, 08, 10, 03, 39, 27), Action = "Deleted"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-79876", CreatedDate = new DateTime(2020, 08, 09, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-81742", CreatedDate = new DateTime(2020, 08, 08, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-11749", CreatedDate = new DateTime(2020, 08, 07, 03, 39, 27), Action = "New"    },
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-12951", CreatedDate = new DateTime(2020, 08, 06, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-38153", CreatedDate = new DateTime(2020, 08, 05, 03, 39, 27), Action = "Deleted"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-79855", CreatedDate = new DateTime(2020, 08, 04, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-81757", CreatedDate = new DateTime(2020, 08, 03, 03, 39, 27), Action = "Updated"},
                new DEA_KDWS_GPlusContributorsLog {forfatterID = "0-81757", CreatedDate = new DateTime(2019, 08, 03, 03, 39, 27), Action = "Deleted"},
            };
            var contributorsDbSet = new FakeDbSet<DEA_KDWS_GPlusContributors>
            {
                new DEA_KDWS_GPlusContributors {contributor_id = "0-11745", contributor_langbeskrivelse = ""},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-12994", contributor_langbeskrivelse = "Description"},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-38145", contributor_langbeskrivelse = "Description"},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-79876", contributor_langbeskrivelse = null},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-81742", contributor_langbeskrivelse = "Description"},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-11749", contributor_langbeskrivelse = ""},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-12951", contributor_langbeskrivelse = "Description"},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-38153", contributor_langbeskrivelse = "Description"},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-79855", contributor_langbeskrivelse = null},
                new DEA_KDWS_GPlusContributors {contributor_id = "0-81757", contributor_langbeskrivelse = "Description"},
            };

            var consolidatedContributorsLogViews = new FakeDbSet<DEA_KDWS_GPlus_ConsolidatedContributorsLogView>
            {
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-11745", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-12994", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-38145", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-79876", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-81742", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Deleted", ContributorId = "0-11749", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Updated", ContributorId = "0-12951", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Updated", ContributorId = "0-38153", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Updated", ContributorId = "0-79855", Timestamp = DateTime.Now},
                new DEA_KDWS_GPlus_ConsolidatedContributorsLogView {Action = "Updated", ContributorId = "0-81757", Timestamp = DateTime.Now}
            };

            KdEntities.Setup(x => x.DEA_KDWS_GPlusproductcontributorsLog).Returns(productContributorsLogDbSet);

            KdEntities.Setup(x => x.DEA_KDWS_GPlusContributorsLog).Returns(contributorsLogsDbSet);

            KdEntities.Setup(x => x.DEA_KDWS_GPlusContributors).Returns(contributorsDbSet);

            KdEntities.Setup(x => x.DEA_KDWS_GPlus_ConsolidatedContributorsLogView).Returns(consolidatedContributorsLogViews);
        }
    }
}