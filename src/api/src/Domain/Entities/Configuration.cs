namespace Domain
{
    public class Configuration
    {
        public Guid Id { get; set; }
        public List<string> AvailableEnvironments { get; set; }
        public List<Location> AvailableLocations { get; set; }
        public List<string> MandatoryTags { get; set; }
    }


    public class Location
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
    }
}
