using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Gyldendal.Api.CoreData.Common.ConfigurationManager
{
    /// <summary>
    /// Provides utility methods to read and cast config values into specific data types.
    /// </summary>
    public class BaseConfigurationManager
    {
        protected static int GetConfigValue(string key, int defaultVal)
        {
            int keyValue;

            if (int.TryParse(GetConfigValue(key), out keyValue))
            {
                return keyValue;
            }
            return defaultVal;
        }

        protected static TimeSpan GetConfigValue(string key, TimeSpan defaultValue)
        {
            TimeSpan timeSpan;
            if (TimeSpan.TryParseExact(GetConfigValue(key), "hh\\:mm\\:ss", CultureInfo.InvariantCulture, out timeSpan))
            {
                return timeSpan;
            }
            return defaultValue;
        }

        protected static DateTime GetConfigValue(string key, DateTime defaultValue)
        {
            var retVal = defaultValue;
            var keyValue = GetConfigValue(key);

            if (!(string.IsNullOrWhiteSpace(keyValue)))
            {
                try
                {
                    long fileDateTime;
                    if (long.TryParse(keyValue, out fileDateTime))
                    {
                        retVal = DateTime.FromFileTimeUtc(fileDateTime).ToLocalTime();
                    }
                }
                catch
                {
                    // ignored
                }
            }

            return retVal;
        }

        protected static IEnumerable<string> GetConfigValuesListFromCsvString(string key)
        {
            return GetConfigValue(key)?.Split(new[] { "," }, StringSplitOptions.None);
        }

        protected static IEnumerable<int> GetNumericConfigValuesFromCsvString(string key)
        {
            var valuesList = GetConfigValuesListFromCsvString(key);

            var retVal = new List<int>();

            valuesList?.ToList().ForEach(x =>
            {
                int id;
                if (int.TryParse(x.Trim(), out id))
                {
                    retVal.Add(id);
                }
            });

            return retVal;
        }

        protected static string GetConfigValue(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key];
        }


        protected static int GetConfigValueAsInt(string key, int defaultVal = 0)
        {
            int keyValue;

            if (int.TryParse(GetConfigValue(key), out keyValue))
            {
                return keyValue;
            }

            return defaultVal;
        }

        protected static bool GetConfigValueAsBool(string key, bool defaultValue = false)
        {
            bool keyValue;

            if (bool.TryParse(GetConfigValue(key), out keyValue))
            {
                return keyValue;
            }

            return defaultValue;
        }
    }
}