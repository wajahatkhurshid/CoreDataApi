namespace Gyldendal.Api.CoreData.Common.Utils
{
    public static class 
        MiscUtils
    {
        public static int ToInt(this string str, int defaultVal = 0)
        {
            int outInt;
            return int.TryParse(str, out outInt) ? outInt : defaultVal;
        }

        public static int? ToIntNullable(this string str)
        {
            int outInt;
            return int.TryParse(str, out outInt) ? outInt : (int?)null;
        }
    }
}
