using ExampleAuthServer.Core.Configuration;
using ExampleAuthServer.Core.Models;
using ExampleAuthServer.Core.Repositories;
using ExampleAuthServer.Core.Services;
using ExampleAuthServer.Core.UnitOfWork;
using ExampleAuthServer.Data;
using ExampleAuthServer.Data.Repositories;
using ExampleAuthServer.Service.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SharedLibrary.Configurations;
using SharedLibrary.Extentions;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleAuthServer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //DI Register 
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IServiceGeneric<,>), typeof(ServiceGeneric<,>)); // 2 parametre aldýðý için <,> yazdýk.
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("SqlServer"), sqloptions =>
                 {
                     sqloptions.MigrationsAssembly("ExampleAuthServer.Data");
                     //Migration oluþacak katmaný belirtiyoruz.Çünkü Migration DbContext'in olunduðu katmanda yapýlýr.
                 });
            });
            services.AddIdentity<UserApp, IdentityRole>(opt =>
            //Identity Role üzerinden miras alan bir class üzerinden rol manuel olarak da yapýlabilir.
            {
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            //Options Pattern = DI üzerinden AppSettingsteki datalara eriþime options pattern adý verilir.
            services.Configure<CustomTokenOption>(Configuration.GetSection("TokenOption"));
            services.Configure<List<Client>>(Configuration.GetSection("Clients"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //Farklý sayýlarda giriþ varsa farklý schemalar kullanýlabilir.
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//Default þemayla alttaki þemanýn haberleþmesini saðlar.

            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var tokenOptions = Configuration.GetSection("TokenOption").Get<CustomTokenOption>();
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
                    ClockSkew = TimeSpan.Zero // Default olarak farklý serverlar arasýndaki zamansal farký tolere etmek için 5 dakika ekler.
                                              // Bunu yazýnca sýfýrlar.
                };
            });

            services.AddControllers().AddFluentValidation(options =>
            {
                options.RegisterValidatorsFromAssemblyContaining<Startup>();
                //Fluent Validation'ýn kendi þemasý var.Biz kendi response sýnýfýmýzý dönmek istiyoruz.CustomValidationResponse adlý extensioný yazýyoruz.
                //SharedLibrary -> Extentions içinde.
            });
            services.UseCustomValidationResponse(); // Fluent validation'ý ezen metot.Extention olarak yazýldý detaylarý içinde.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ExampleAuthServer.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExampleAuthServer.API v1"));
            }
            else
            {
            }

            app.UseCustomException();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
