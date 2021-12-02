using FinDoxDocumentsAPI.Repositories;
using FinDoxDocumentsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FinDoxDocumentsAPI
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = Configuration["Jwt:Audience"],
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                }
            );

            var dbConnectionString = Configuration.GetConnectionString("dbConnection");
            var dbDocumentConnectionString = Configuration.GetConnectionString("dbDocumentConnection");

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

            services.AddSingleton<IDbConnectionFactory> ((sp) => new DbConnectionFactory(dbConnectionString, dbDocumentConnectionString));

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IGroupRepository, GroupRepository>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IDocumentService, DocumentService>();

            services.AddControllers().AddNewtonsoftJson();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseErrorMiddleware();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseWhen(context => !context.Request.Path.StartsWithSegments("/api/token"), appBuilder => appBuilder.UseUserMiddleware());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
