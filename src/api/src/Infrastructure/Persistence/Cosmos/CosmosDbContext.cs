using Domain.Resources;
using Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Domain.Deployments;
using Newtonsoft.Json.Linq;
using Domain;

namespace Infrastructure.Persistance.Cosmos
{
    public class CosmosDbContext : DbContext
    {
        private readonly IOptions<CosmosDbContextOptions> _projectDbContextOptions;
        public CosmosDbContext(DbContextOptions options, IOptions<CosmosDbContextOptions> projectDbContextOptions) : base(options)
        {
            _projectDbContextOptions = projectDbContextOptions;
        }

        protected CosmosDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasNoDiscriminator()
                .ToContainer(_projectDbContextOptions.Value.ProjectsContainerName)
                .HasPartitionKey(x => x.Id)
                .Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<Configuration>()
                .HasNoDiscriminator()
                .ToContainer(_projectDbContextOptions.Value.ConfigurationContainerName)
                .HasPartitionKey(x => x.Id)
                .Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<Project>()
                .Property(x => x.MandatoryTags).HasConversion(
                   v => JObject.FromObject(v),
                   v => v.ToObject<Dictionary<string, string>>()
                );

            modelBuilder.Entity<DeploymentLog>()
               .HasNoDiscriminator()
               .ToContainer(_projectDbContextOptions.Value.DeploymentLogsContainerName)
               .HasPartitionKey(x => x.Id)
               .Property(x => x.Id).ToJsonProperty("id");


            modelBuilder.Entity<Resource>()
                .HasNoDiscriminator()
                .ToContainer(_projectDbContextOptions.Value.ResourcesContainerName)
                .HasPartitionKey(x => x.Id)
                .Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<Domain.Applications.Application>()
               .HasNoDiscriminator()
               .ToContainer(_projectDbContextOptions.Value.ApplicationsContainerName)
               .HasPartitionKey(x => x.Id)
               .Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<Domain.Applications.Application>()
                .Property(x => x.SettingsFiles).HasConversion(
                   v => new List<string>(v),
                   v => v
                );

            modelBuilder.Entity<Deployment>()
               .HasNoDiscriminator()
               .ToContainer(_projectDbContextOptions.Value.DeploymentsContainerName)
               .HasPartitionKey(x => x.Id)
               .Property(x => x.Id).ToJsonProperty("id");

            modelBuilder.Entity<Deployment>()
               .HasNoDiscriminator()
               .ToContainer(_projectDbContextOptions.Value.DeploymentsContainerName)
               .HasPartitionKey(x => x.Id)
               .Property(x => x.Status).HasConversion<string>();

            modelBuilder.Entity<Deployment>()
                .OwnsMany(x => x.EnvironmentDeployments)
                .OwnsMany(x => x.ApplicationIdentityDeployments)
                .Property(x => x.AuthorizedApps).HasConversion(
                   v => new List<string>(v),
                   v => v
                );

            modelBuilder.Entity<Deployment>()
                .OwnsMany(x => x.EnvironmentDeployments)
                .OwnsMany(x => x.ApplicationIdentityDeployments)
                .Property(x => x.Type).HasConversion<string>();

            modelBuilder.Entity<Deployment>()
               .OwnsMany(x => x.EnvironmentDeployments)
               .OwnsMany(x => x.ApplicationIdentityDeployments)
               .Property(x => x.RedirectUris).HasConversion(
                   v => new List<string>(v),
                   v => v
                );


            modelBuilder.Entity<Deployment>()
               .OwnsMany(x => x.EnvironmentDeployments)
               .OwnsMany(x => x.ResourceDeployments)
               .Property(x => x.Parameters).HasConversion(
                   v => JObject.FromObject(v),
                   v => v.ToObject<Dictionary<string, ParameterValue>>()
               );

            modelBuilder.Entity<Deployment>()
              .OwnsMany(x => x.EnvironmentDeployments)
              .OwnsOne(x => x.ResourceGroup)
              .Property(x => x.Tags).HasConversion(
                  v => JObject.FromObject(v),
                  v => v.ToObject<Dictionary<string, string>>()
              );

            modelBuilder.Entity<Deployment>()
                .OwnsMany(x => x.CodeDeployments)
                .OwnsOne(x => x.RepositoryDeployment)
                .Property(x => x.SettingsFilesNames).HasConversion(
                   v => new List<string>(v),
                   v => v
                );
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Domain.Applications.Application> Applications { get; set; }
        public DbSet<Deployment> Deployments { get; set; }
        public DbSet<DeploymentLog> DeploymentLogs { get; set; }

        public DbSet<Configuration> Configurations { get; set; }
    }
}
