namespace Domain.Projects
{
    public class Project
    {
        protected Project() {}

        public Project(Guid id, DateTimeOffset createdDate, string name, string createdBy, string description)
        {
            Id = id;
            Created = createdDate;
            Name = name;
            CreatedBy = createdBy;
            Description = description;
        }

        public Guid Id { get; set; }
        public string Name { get; protected set; }
        public DateTimeOffset Created { get; init; }
        public DateTimeOffset? Updated { get; protected set; }
        public string CreatedBy { get; init; }
        public string UpdatedBy { get; protected set; }
        public string Description { get; protected set; }
        public string Diagram { get; protected set; }
        public List<string> Environments { get; protected set; }
        public string ResourceGroup { get; protected set; }
        public IDictionary<string, string> MandatoryTags { get; protected set; }
        public string Location { get; protected set; }

        public void Edit(
            string name, 
            string description, 
            string diagram, 
            DateTimeOffset updatedTime, 
            string updatedBy, 
            List<string> environments, 
            string resourceGroup, 
            string location, 
            IDictionary<string, string> mandatoryTags)
        {
            Name = name;
            Description = description;
            Updated = updatedTime;
            UpdatedBy = updatedBy;
            Diagram = diagram;
            Environments = environments;
            ResourceGroup = resourceGroup;
            Location = location;
            MandatoryTags = mandatoryTags;
        }
    }
}
