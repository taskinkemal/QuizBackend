using System;

namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class Convertor
    {
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
