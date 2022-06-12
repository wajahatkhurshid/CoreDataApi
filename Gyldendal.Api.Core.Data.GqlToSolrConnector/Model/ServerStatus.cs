// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    /// <summary>
    /// Status of SolrServer and core
    /// </summary>
    public class ServerStatus
    {
        public bool IsCoreExists { get; set; }
        public bool IsServerRunning { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }
    }
}
