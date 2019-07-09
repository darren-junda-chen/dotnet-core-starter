using Domain.Settings;
using Domain.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Api.Middlewares;
using Business.Managers;
using AutoMapper;
using Business.Mappings;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Business.Engines;
using Business.Helpers;

namespace Api
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
      services.AddCors();

      services.AddMvc(config =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        config.Filters.Add(new AuthorizeFilter(policy));
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      .AddJsonOptions(options =>
      {
        options.SerializerSettings.ContractResolver = new CamelCaseGetOnlyContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      });

      // Setup configurations
      var generalSettings = Configuration.GetSection("General").Get<GeneralSettings>();
      services.AddSingleton<GeneralSettings>(generalSettings);

      // Setup db connection
      services.AddDbContext<MainContext>(options => options.UseNpgsql(generalSettings.MainDbConnectionString));

      // Setup auto mapper
      services.AddAutoMapper(config => config.AddProfiles(typeof(MappingProfiles).GetTypeInfo().Assembly));

      // Add http client
      services.AddHttpClient();

      // Setup dependencies from business layer
      SetupBusinessDependencies(services);

      // Setup authentication 
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
          {
            options.TokenValidationParameters = new TokenValidationParameters
            {
              ValidateIssuer = false,
              ValidateAudience = false,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(generalSettings.JwtKey))
            };
          });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, MainContext mainContext)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseHsts();
      }

      app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
      app.UseHttpsRedirection();
      app.UseMiddleware(typeof(ErrorHandlingMiddleware));
      app.UseAuthentication();
      app.UseMvc();
    }

    private void SetupBusinessDependencies(IServiceCollection services)
    {
      // Engines
      services.AddScoped<IPasswordHashEngine, PasswordHashEngine>();
      services.AddScoped<ILogEngine, LogEngine>();

      // Managers
      services.AddScoped<IAuthorisationManager, AuthorisationManager>();
      services.AddScoped<IMembershipsManager, MembershipsManager>();
    }
  }
}
