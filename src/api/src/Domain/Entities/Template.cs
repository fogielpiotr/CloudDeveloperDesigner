namespace Domain.Templates
{
    public class Template
    {
        public IDictionary<string, TemplateParameter> Parameters { get; set; }
    }

    public class TemplateParameter
    {
        public TemplateParameter(string type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Type { get; set; }
        public IEnumerable<string> AllowedValues { get; set; }
        public string DefaultValue { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public int? MinLenght { get; set; }
        public int? MaxLenght { get; set; }
    }
}

