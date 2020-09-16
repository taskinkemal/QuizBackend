using System;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Convertor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool ToBoolean(object obj, bool defaultValue)
        {
            if (obj != null && obj != DBNull.Value)
            {
                bool result;
                if (bool.TryParse(obj.ToString(), out result))
                {
                    return result;
                }
                else
                {
                    var tmp = ToInt32(obj, -1);

                    if (tmp == 1)
                    {
                        return true;
                    }

                    if (tmp == 0)
                    {
                        return false;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short ToInt16(object obj, short defaultValue)
        {
            if (obj != null && obj != DBNull.Value)
            {
                var s = obj.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (short.TryParse(s, out var result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt32(object obj, int defaultValue)
        {
            if (obj != null && obj != DBNull.Value)
            {
                var s = obj.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (int.TryParse(s, out var result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double ToDouble(object obj, double defaultValue)
        {
            if (obj != null && obj != DBNull.Value)
            {
                var s = obj.ToString();
                if (!string.IsNullOrWhiteSpace(s))
                {
                    if (double.TryParse(s, out var result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(object obj, DateTime defaultValue, string format = null)
        {
            if (obj != null && obj != DBNull.Value)
            {
                DateTime result;

                if (!string.IsNullOrWhiteSpace(format))
                {
                    if (DateTime.TryParseExact(obj.ToString(), format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                    {
                        return result;
                    }
                }
                else
                {
                    if (DateTime.TryParse(obj.ToString(), out result))
                    {
                        return result;
                    }
                }
            }

            return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToString(object obj, string defaultValue)
        {
            return obj != null && obj != DBNull.Value ? obj.ToString() : defaultValue;
        }
    }
}
