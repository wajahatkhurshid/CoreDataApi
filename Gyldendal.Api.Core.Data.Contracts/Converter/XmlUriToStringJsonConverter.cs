using System;
using System.Globalization;

namespace Gyldendal.Api.CoreData.Contracts.Converter
{
    public class XmlUriToStringJsonConverter : CustomJsonConverter<DateTime, string>
    {
        protected override string Convert(DateTime value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override DateTime Convert(string value)
        {
            return DateTime.Parse(value);
        }
    }
}
