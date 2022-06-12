using System.IO;
using System.Xml.Serialization;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Common.Utils
{
    /// <summary>
    /// Utility class for serialization/deserialization of objects.
    /// </summary>
    public static class SerializerUtil
    {
        /// <summary>
        /// Serializes the data in the object to the designated file path
        /// </summary>
        /// <typeparam name="T">Type of Object to serialize</typeparam>
        /// <param name="dataToSerialize">Object to serialize</param>
        /// <returns>XML string containing serialized information</returns>
        [Trace]
        public static string Serialize<T>(T dataToSerialize)
        {
            var serializer = new XmlSerializer(typeof(T));
            var writer = new StringWriter();
            serializer.Serialize(writer, dataToSerialize);
            writer.Close();

            return writer.ToString();
        }

        /// <summary>
        /// Deserializes the data in the XML file into an object
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize</typeparam>
        /// <param name="dataToDeserialize"></param>
        /// <returns>Object containing deserialized data</returns>
        [Trace]
        public static T Deserialize<T>(string dataToDeserialize)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            var textReader = new StringReader(dataToDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }
    }
}
