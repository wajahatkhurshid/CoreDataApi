using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Contracts.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;

namespace Gyldendal.Api.CoreData.Tests.ContributorRepository.GDP
{
    using Business.Repositories.GPlus;

    [TestClass]
    public class ContributorRepositoryTest
    {
        private Mock<ContributorRepository> _contributorRepositoryMock;

        private Mock<IProductDataProvider> _productDataProvider;

        private Mock<IContentfulManager> _contentfulManager;

        private Mock<ILogger> _logger;

        [TestInitialize]
        public void Setup()
        {
            ContributorFactory.Setup();
            _productDataProvider = new Mock<IProductDataProvider>();
            _contentfulManager = new Mock<IContentfulManager>();
            _logger = new Mock<ILogger>();
            _contributorRepositoryMock = new Mock<ContributorRepository>(ContributorFactory.KdEntities.Object, _productDataProvider.Object, _contentfulManager.Object, _logger.Object);
        }

        [TestMethod]
        public void ContributorCount_TotalCountEqualsTotalInPaging()
        {
            var count = _contributorRepositoryMock.Object.GetUpdatedContributorsCount(DateTime.MinValue);
            var res = _contributorRepositoryMock.Object.GetContributorsUpdateInfo(DateTime.MinValue, 0, count);
            Assert.AreEqual(count, res.Count());
        }

        [TestMethod]
        public void VerifyContributorIds_MustStartWithZeroOrOne()
        {
            var res = _contributorRepositoryMock.Object.GetContributorsUpdateInfo(DateTime.MinValue, 0, int.MaxValue);
            Assert.IsTrue(res.All(x => x.ContributorId.StartsWith("0-") || x.ContributorId.StartsWith("1-")));
        }

        [TestMethod]
        public void VerifyPageSize_PageSizeMustBeEqualToProvidedPageSize()
        {
            var res = _contributorRepositoryMock.Object.GetContributorsUpdateInfo(DateTime.MinValue, 0, 3);
            Assert.AreEqual(res.Count(), 3);
        }

        [TestMethod]
        public void VerifyDeleted_DeletedCountShouldBeEqualToDeletedPlusContributorsWithEmptyDescriptions()
        {
            const int deleted = 6;

            var res = _contributorRepositoryMock.Object.GetContributorsUpdateInfo(DateTime.MinValue, 0, 10);
            Assert.AreEqual(res.Count(x => x.UpdateType == ContributorUpdateType.Deleted), deleted);
        }

        [TestMethod]
        public void VerifyUpdateType_UpdateTypeMustBeOfTheLatestChange()
        {
            var res = _contributorRepositoryMock.Object.GetContributorsUpdateInfo(DateTime.MinValue, 0, int.MaxValue);
            var selectedContributor = res.First(x => x.ContributorId == "0-81757");
            Assert.AreEqual(selectedContributor.UpdateType, ContributorUpdateType.Updated);
        }
    }
}