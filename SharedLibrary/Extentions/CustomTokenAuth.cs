using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extentions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenOption tokenOptions)
        {//Token Options DI olarak startupta tanımlıyoruz ve appsettings.jsonda tutuyoruz.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //Farklı sayılarda giriş varsa farklı schemalar kullanılabilir.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//Default şemayla alttaki şemanın haberleşmesini sağlar.

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),
                    //  Buraya kadar olanlar appsettings.json içinden geliyor.
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    //Hepsi kontrol ediliyor.
                    ClockSkew = TimeSpan.Zero // Default olarak farklı serverlar arasındaki zamansal farkı tolere etmek için 5 dakika ekler.
                                              // Bunu yazınca sıfırlar.
                };
            });
        }
    }
}
