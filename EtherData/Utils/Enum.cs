using System;

namespace EtherData.Utils
{
    public static class EnumExtentions
    {
        public static string ToKey(this Enum soure, string format)
        {
            return string.Format(format, (int)(IConvertible)soure);
        }
    }

    public class Enum<T> where T : struct, IConvertible
    {
        public static T Parse(string value)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            if (!string.IsNullOrEmpty(value)
                && Enum.TryParse(value, out T res)
                && Enum.IsDefined(typeof(T), res))
            {
                return res;
            }
            return default(T);
        }
    }
}
