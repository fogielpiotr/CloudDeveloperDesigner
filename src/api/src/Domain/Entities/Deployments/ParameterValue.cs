namespace Domain.Deployments
{
    public class ParameterValue
    {
        public ParameterValue(string value)
        {
            Value = value;
        }

        public string Value { get; init; }
    }
}
