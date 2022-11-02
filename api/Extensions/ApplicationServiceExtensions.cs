using api.Data;
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
            Services.AddDbContext<DataContext>(options => {
                options.UseSqlite(Config.GetConnectionString("DefaultConnection"));
            });
            Services.AddScoped<IUserToken, TokenService>();
            return Services;
        }
    }
}
