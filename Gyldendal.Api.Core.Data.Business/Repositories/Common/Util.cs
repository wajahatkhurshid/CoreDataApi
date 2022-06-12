using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.IO;
using System.Text;
using TidyManaged;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public static class Util
    {
        /// <summary>
        /// Converts the given action value to equivalent enum value of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">If action value is invalid.</exception>
        public static T GetUpdateType<T>(this string action) where T : struct, IComparable
        {
            if (string.IsNullOrEmpty(action))
                return default(T);

            Enum.TryParse(action, true, out T result);
            return result;
        }

        /// <summary>
        /// Converts the given action value to equivalent ProductUpdateType enum value.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">If action value is invalid.</exception>
        public static ProductUpdateType ToProductUpdateType(this string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException($"Value of {nameof(action)} cannot be empty.");
            }

            ProductUpdateType retVal;
            if (!(Enum.TryParse(action, out retVal)))
            {
                throw new ArgumentException($"Value {action} is invalid for the parameter {nameof(action)}.");
            }

            return retVal;
        }

        /// <summary>
        /// Converts the given action value to equivalent ProductUpdateType enum value.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">If action value is invalid.</exception>
        public static ContributorUpdateType ToContributorUpdateType(this string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException($"Value of {nameof(action)} cannot be empty.");
            }

            ContributorUpdateType retVal;
            if (!(Enum.TryParse(action, out retVal)))
            {
                throw new ArgumentException($"Value {action} is invalid for the parameter {nameof(action)}.");
            }

            return retVal;
        }

        /// <summary>
        /// Converts the given action value to equivalent ProductType enum value.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">If action value is invalid.</exception>
        public static ProductType ToProductType(this string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentException($"Value of {nameof(action)} cannot be empty.");
            }

            switch (action.Trim().ToUpper())
            {
                case "V":
                    return ProductType.SingleProduct;

                case "B":
                    return ProductType.Bundle;

                default:
                    return ProductType.None;
            }
        }

        /// <summary>
        /// Repair html and return valid html string.
        /// </summary>
        /// <param name="brokenText">Text with html tags</param>
        /// <returns></returns>
        public static string RepairHtml(this string brokenText)
        {
            if (string.IsNullOrWhiteSpace(brokenText))
                return null;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(brokenText);
                using (MemoryStream inputStream = new MemoryStream(byteArray))
                {
                    using (Document doc = Document.FromStream(inputStream))
                    {
                        doc.DocType = DocTypeMode.Omit;
                        doc.OutputBodyOnly = AutoBool.Yes;
                        doc.CharacterEncoding = EncodingType.Utf8;
                        doc.InputCharacterEncoding = EncodingType.Utf8;
                        doc.OutputCharacterEncoding = EncodingType.Utf8;
                        doc.MaximumErrors = int.MaxValue;
                        doc.CleanAndRepair();

                        using (MemoryStream outStream = new MemoryStream())
                        {
                            doc.Save(outStream);
                            outStream.Position = 0;
                            using (StreamReader sr = new StreamReader(outStream))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // todo: Log exception.
                return brokenText;
            }
        }
    }
}