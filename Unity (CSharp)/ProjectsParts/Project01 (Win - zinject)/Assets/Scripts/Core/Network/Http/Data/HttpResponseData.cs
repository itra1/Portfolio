namespace Core.Network.Http.Data
{
    public struct HttpResponseData<TResult> where TResult : class
    {
        public TResult Result { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsFailed => !string.IsNullOrEmpty(ErrorMessage);
    }
}