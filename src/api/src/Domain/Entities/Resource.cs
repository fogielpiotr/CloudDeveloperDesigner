using System.ComponentModel.DataAnnotations;

namespace Domain.Resources
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string TemplateFileName { get; set; }
        public string DisplayName { get; set; }
        public string IconFile { get; set; }
        public bool IsSecretProvider { get; set; }
        public ResourceNaming ResourceNaming { get; set; }
    }

    public class ResourceNaming
    {
        public string Abbreviation { get; set; }
        public string Divider { get; set; }
        public int MinLenght { get; set; }
        public int MaxLenght { get; set; }
    }
}
