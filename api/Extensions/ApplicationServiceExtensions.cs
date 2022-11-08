using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Services;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices (this IServiceCollection Services , IConfiguration Config)
        {
            Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            Services.AddScoped<IUserToken, TokenService>();
            Services.AddScoped<IUserRepository, UserRepository>();
            Services.AddDbContext<DataContext>(options => {
                options.UseSqlite(Config.GetConnectionString("DefaultConnection"));
            });
            return Services;
        }
    }
}
