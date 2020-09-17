namespace Models.TransferObjects
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TE"></typeparam>
    public class GenericManagerResponse<T, TE> where TE : System.Enum where T : class
    {
        /// <summary>
        /// 
        /// </summary>
        public TE Response { get; }

        /// <summary>
        /// 
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <param name="value"></param>
        public GenericManagerResponse(TE response, T value)
        {
            Response = response;
            Value = value;
        }
    }
}
