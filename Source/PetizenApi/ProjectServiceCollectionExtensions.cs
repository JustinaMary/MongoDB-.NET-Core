namespace PetizenApi
{
    using GraphQL;
    using Microsoft.Extensions.DependencyInjection;
    using PetizenApi.Interfaces;
    using PetizenApi.Providers;
    using PetizenApi.Repositories;
    using PetizenApi.Schemas;
    using PetizenApi.Services;


    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods add project services.
    /// </summary>
    /// <remarks>
    /// AddSingleton - Only one instance is ever created and returned.
    /// AddScoped - A new instance is created and returned for each request/response cycle.
    /// AddTransient - A new instance is created and returned each time.
    /// </remarks>
    internal static class ProjectServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) =>
            services
                .AddSingleton<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService))
                .AddSingleton<IClockService, ClockService>()
                .AddSingleton<ITokenGenerator, TokenGenerator>()
                .AddSingleton<IConnectionFactory, ConnectionFactory>();




        /// <summary>
        /// Add project data repositories.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The services with caching services added.</returns>
        public static IServiceCollection AddProjectRepositories(this IServiceCollection services) =>
            services

            .AddTransient<IAccountRepository, AccountRepository>()
             .AddSingleton<ContextServiceLocator>()
            .AddSingleton<IDogRepository, DogRepository>()
            .AddSingleton<IEmailRepository, EmailRepository>()
            .AddSingleton<ICommonRepository, CommonRepository>()
            .AddSingleton<IConnectionFactory, ConnectionFactory>()
            .AddSingleton<IEmailRepository, EmailRepository>()
            .AddSingleton<ITokenGenerator, TokenGenerator>()

            .AddSingleton<IDogRepository, DogRepository>()
            .AddSingleton<ILocationServiceRepository, LocationServiceRepository>()
            .AddSingleton<UploadRepository>()
            .AddSingleton<IMenuRepository, MenuRepository>()
            .AddSingleton<ICommonRepository, CommonRepository>()
            .AddSingleton<IChatRepository, ChatRepository>()
            .AddSingleton<ITicketingRepository, TicketingRepository>();

        /// <summary>
        /// Add project GraphQL schema and web socket types.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>The services with caching services added.</returns>
        public static IServiceCollection AddProjectSchemas(this IServiceCollection services) =>
            services
                .AddSingleton<MainSchema>();

    }
}
