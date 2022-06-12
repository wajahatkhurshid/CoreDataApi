using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gyldendal.Api.CoreData.Contracts.Converter
{
    /// <summary>
    /// Use this to solve the issue of Json Parsing the XmlUri contract.
    /// Source: http://stackoverflow.com/questions/16948626/serialize-complex-type-as-simple-type-json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public abstract class CustomJsonConverter<T, TResult> : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            T typedValued = (T)value;
            TResult valueToSerialize = Convert(typedValued);

            serializer.Serialize(writer, valueToSerialize);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(T) == objectType;
        }

        protected virtual T Create(Type type, JObject jObject)
        {
            var simpleValue = jObject.Value<TResult>();
            return Convert(simpleValue);
        }

        protected abstract TResult Convert(T value);
        protected abstract T Convert(TResult value);
    }
}
