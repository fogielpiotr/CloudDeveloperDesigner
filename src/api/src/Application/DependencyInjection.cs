using Application.Common.Interfaces;
using Application.Common.Services;
using Application.Configuration.Query.GetConfigurations;
using Application.PipelineBehaviors;
using Domain.Interfaces;
using Domain.Services;
using Domain.Services.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddSingleton<IClock, Clock>();

            services.AddTransient<IDeploymentEventService, DeploymentEventService>();
            services.AddScoped<ICodeDeploymentService, CodeDeploymentService>();
            services.AddScoped<IResourceDeploymentService, ResourceDeploymentService>();
            services.AddScoped<IApplicationIdentityService, ApplicationIdentityService>();
            services.AddScoped<IEnvironmentDeploymentService, EnvironmentDeploymentService>();

            return services;
        }
    }
}