using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Common.Request;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace Gyldendal.Api.CoreData.Business.Repositories.Global
{
    public class CoreDataAgentRepository : ICoreDataAgentRepository
    {
        private readonly IConfigurationManager _configurationManager;

        public CoreDataAgentRepository(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public object GetCoreDataAgentImportStates(ImportStates importStates)
        {
            try
            {
                var xmlFile = Path.Combine(_configurationManager.CoreDataAgentImportStateFilesPath, $"{importStates:G}.xml");

                var xml = File.ReadAllText(xmlFile);
                // Removing Xml node and namespaces
                xml = Regex.Replace(xml, "<\\?xml.*>", "", RegexOptions.IgnoreCase);
                xml = Regex.Replace(xml, "xmlns.*>", ">", RegexOptions.IgnoreCase);
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var json = JsonConvert.SerializeXmlNode(doc);
                var importStateObject = JsonConvert.DeserializeObject<object>(json);
                return importStateObject;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}