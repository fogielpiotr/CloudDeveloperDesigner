namespace Domain.Deployments
{
    public abstract class DeploymentObject<T> 
    {
        public string Name { get; set; }
        public T CloudIdentifier { get; private set; }
        public string Url { get; private set; }
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }

        public virtual void SetSuccess(CloudCreatorResponse<T> response)
        {
            Url = response.Url;
            CloudIdentifier = response.CloudIdentifier;
            Success = true;
        }

        public virtual void SetFailure(string errorMessage)
        {
            Success = false;
            ErrorMessage = errorMessage;
        }
    }
}
