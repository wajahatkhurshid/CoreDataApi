using System.Xml.Serialization;

namespace Gyldendal.Api.CoreData.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    
    [XmlType("RefEntity")]
    public class RefEntity
    {
        /// <summary>
        /// Code of Ref Entity
        /// </summary>
        [XmlElement("Code")]
        public int Code { get; set; }
        /// <summary>
        /// Display Name of Ref Entity
        /// </summary>
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }
    }
}
