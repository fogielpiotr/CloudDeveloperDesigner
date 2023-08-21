namespace Domain.Deployments
{
    public class CloudCreatorResponse<T>
    {
        public CloudCreatorResponse(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }

        public CloudCreatorResponse(T value, string url)
        {
            Success = true;
            CloudIdentifier = value;
            Url = url;
        }

        public bool Success { get; init; }
        public T CloudIdentifier { get; init; }
        public string Url { get; init; }
        public string ErrorMessage { get; init; }
    }
}
