namespace SiteBlue.Business
{
    public class OperationResult<T>
    {
        //internal constructor to prevent spoofing business
        //layer responses.
        internal OperationResult() {}

        public bool Success { get; set; }
        public T ResultData { get; set; }
        public string Message { get; set; }
    }
}
