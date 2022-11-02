using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace api.Extensions
{
    public static class IdentityServiceExtensions
    {


        public static IServiceCollection AddIdentityServices (this IServiceCollection Services, IConfiguration Config)
        {
            Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["TokenKey"])),
                ValidateIssuer = false,
                ValidateAudience = false
            }

            );
            return Services;
        }
            
            }
}
