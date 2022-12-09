using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Services;
using api.SignalR;

using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,
            IConfiguration config)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            services.AddCors();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped <logUserActivity>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        
            services.AddSignalR();
            services.AddSingleton<PresenceTracker>();

            return services;
        }
    }
}