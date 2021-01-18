namespace PetizenApi
{
    using Boxed.AspNetCore;
    using GraphQL.Server;
    using GraphQL.Server.Ui.Playground;
    using GraphQL.Server.Ui.Voyager;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using PetizenApi.Constants;
    using PetizenApi.Schemas;
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    /// The main start-up class for the application.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The application configuration, where key value pair settings are stored (See
        /// http://docs.asp.net/en/latest/fundamentals/configuration.html).</param>
        /// <param name="webHostEnvironment">The environment the application is running under. This can be Development,
        /// Staging or Production by default (See http://docs.asp.net/en/latest/fundamentals/environments.html).</param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            this.configuration = configuration;
            this.webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Configures the services to add to the ASP.NET Core Injection of Control (IoC) container. This method gets
        /// called by the ASP.NET runtime (See
        /// http://blogs.msdn.com/b/webdev/archive/2014/06/17/dependency-injection-in-asp-net-vnext.aspx).
        /// </summary>
        /// <param name="services">The services.</param>


        /// <summary>
        /// Configures the application and HTTP request pipeline. Configure is called after ConfigureServices is
        /// called by the ASP.NET runtime.
        /// </summary>
        /// <param name="application">The application builder.</param>
        /// 

        public virtual void ConfigureServices(IServiceCollection services)
        {

            services.AddDatabaseConfig(this.configuration);
            services.AddIdentityConfig();
            services.AddIdentityPassword();

            services.AddCustomCaching();
            services.AddCustomCors();
            services.AddCustomOptions(this.configuration);
            services.AddCustomRouting();
            services.AddCustomResponseCompression(this.configuration);
            services.AddCustomStrictTransportSecurity();
            services.AddCustomHealthChecks();
            services.AddHttpContextAccessor();
            services.AddServerTiming();
            services.AddControllers()
            .AddCustomJsonOptions(this.webHostEnvironment)
            .AddCustomMvcOptions(this.configuration);
            services.AddCustomGraphQL(this.configuration, this.webHostEnvironment);

            services.AddCustomGraphQLAuthorization();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddJWTAuthentication(this.configuration);
            services.AddProjectServices();
            services.AddProjectRepositories();
            services.AddProjectSchemas();
            services.AddConfigureLocalization();
            services.AddLocalizationMethod();
        }


        public virtual void Configure(IApplicationBuilder application)
        {

            application
               .UseIf(
                   this.webHostEnvironment.IsDevelopment(),
                   x => x.UseServerTiming());
            application.UseForwardedHeaders();
            application.UseResponseCompression()
                .UseIf(
                    !this.webHostEnvironment.IsDevelopment(),
                    x => x.UseHsts())
                .UseIf(
                    this.webHostEnvironment.IsDevelopment(),
                    x => x.UseDeveloperExceptionPage());
            application.UseRouting();
            application.UseAuthentication();
            application.UseCors(CorsPolicyName.AllowAny)
            .UseStaticFilesWithCacheControl();
            application.UseCustomSerilogRequestLogging();
            application.UseEndpoints(
                    builder =>
                    {
                        builder
                            .MapHealthChecks("/status")
                            .RequireCors(CorsPolicyName.AllowAny);
                        builder
                            .MapHealthChecks("/status/self", new HealthCheckOptions() { Predicate = _ => false })
                            .RequireCors(CorsPolicyName.AllowAny);
                        builder.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    });


            var supportedCultures = new[] { "en-US", "fr" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            application.UseRequestLocalization(localizationOptions);


            application.UseWebSockets();
            // Use the GraphQL subscriptions in the specified schema and make them available at /graphql.
            application.UseGraphQLWebSockets<MainSchema>();
            // Use the specified GraphQL schema and make them available at /graphql.
            application.UseGraphQL<MainSchema>();
            application.UseIf(
                    this.webHostEnvironment.IsDevelopment(),
                    x => x
                        // Add the GraphQL Playground UI to try out the GraphQL API at /.
                        .UseGraphQLPlayground(new GraphQLPlaygroundOptions() { Path = "/" })
                        // Add the GraphQL Voyager UI to let you navigate your GraphQL API as a spider graph at /voyager.
                        .UseGraphQLVoyager(new GraphQLVoyagerOptions() { Path = "/voyager" }));

        }
    }
}
