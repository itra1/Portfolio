namespace Core.FileResources.Info
{
    public struct ResourceRequestResult<TResource>
    {
        public TResource Resource { get; }
        public string FilePath { get; }
        public string ErrorMessage { get; }
        
        public ResourceRequestResult(TResource resource, string filePath, string errorMessage)
        {
            Resource = resource;
            FilePath = filePath;
            ErrorMessage = errorMessage;
        }
    }
}