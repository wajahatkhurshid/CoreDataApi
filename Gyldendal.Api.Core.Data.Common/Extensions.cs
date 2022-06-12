using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Gyldendal.Common.WebUtils.Models;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Constanct use for gyldendal System name
        /// </summary>
        public const string CoreDataSystemName = "CoreData";

        /// <summary>
        /// Get the the description of the enumeration
        /// </summary>
        /// <param name="value">The enumeration value</param>
        /// <returns>The </returns>
        [Trace]
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        /// <summary>
        /// Parses an int64 to enumeration value
        /// </summary>
        /// <typeparam name="T">The enumeration</typeparam>
        /// <param name="val">The ulong value</param>
        /// <returns>The enumeration value if valid.</returns>
        /// <exception cref="InvalidEnumArgumentException">If value is not valid enumeration value</exception>
        public static T GetEnum<T>(ulong val)
        {
            var enumVal = (T)Enum.ToObject(typeof(T), val);

            if (!Enum.IsDefined(typeof(T), enumVal))
                throw new InvalidEnumArgumentException("Provided value is not valid.");

            return enumVal;

        }

        public static T GetEnum<T>(this string value)
        {
            var enumVal = (T) Enum.ToObject(typeof (T), value);

            if (!Enum.IsDefined(typeof(T), enumVal))
                throw new InvalidEnumArgumentException("Provided value is not valid.");

            return enumVal;
        }

        public static T GetValueFromDescription<T>(this string description) where T : Enum
        {
            var comparison = StringComparison.InvariantCultureIgnoreCase;
            foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (string.Compare(attribute.Description, description, comparison) == 0)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (string.Compare(field.Name, description, comparison) == 0)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Get values of enumerations
        /// </summary>
        /// <typeparam name="T">The enumeration whose values are required</typeparam>
        /// <returns>All values of the enumeration</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Get the the description of the enumeration
        /// </summary>
        /// <param name="value">The enumeration value</param>
        /// <returns>The </returns>
        public static ErrorDetail GetErrorDetail(this ErrorCodes value)
        {
            var description = value.GetDescription();
            return new ErrorDetail
            {
                Description = description,
                Code = (ulong)value,
                OriginatingSystem = CoreDataSystemName
            };
        }
    }
}
