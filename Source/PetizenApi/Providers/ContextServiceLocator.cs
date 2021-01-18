using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PetizenApi.Interfaces;


namespace PetizenApi.Providers
{
    public class ContextServiceLocator
    {
        //public IDogRepository DogRepository => _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IDogRepository>();
        public IAccountRepository AccountRepository => _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IAccountRepository>();

        public IMenuRepository MenuRepository => _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IMenuRepository>();

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ContextServiceLocator(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
