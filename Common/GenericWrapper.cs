using System.Runtime.Serialization;


namespace Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public class GenericWrapper<T>
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public T Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GenericWrapper<T> Wrap(T value)
        {
            return new GenericWrapper<T>
            {
                Value = value
            };
        }
    }
}
