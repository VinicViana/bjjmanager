using BJJManager.Application.Common.Interfaces;
using BJJManager.Infrastructure.Ai;
using BJJManager.Infrastructure.Identity;
using BJJManager.Infrastructure.Persistence;
using BJJManager.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BJJManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BjjManagerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<OpenAiSettings>(configuration.GetSection("OpenAI"));

        services.AddHttpContextAccessor();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITrainingSessionRepository, TrainingSessionRepository>();
        services.AddScoped<ITechniqueRepository, TechniqueRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, EfPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHttpClient<IAiChatClient, OpenAiChatClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.openai.com/v1/");
        });

        return services;
    }
}
