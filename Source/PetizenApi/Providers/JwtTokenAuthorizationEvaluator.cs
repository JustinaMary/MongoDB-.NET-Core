using GraphQL;
using GraphQL.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetizenApi.Providers
{
    public class JwtTokenAuthorizationEvaluator : IAuthorizationEvaluator
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IWebHostEnvironment _environment;
        private readonly AuthorizationSettings _authorizationSettings;

        public JwtTokenAuthorizationEvaluator(IHttpContextAccessor httpContextAccessor,
            AuthorizationSettings authorizationSettings)
        {
            _httpContextAccessor = httpContextAccessor;
            //_environment = environment;
            _authorizationSettings = authorizationSettings;
        }

        public async Task<AuthorizationResult> Evaluate(ClaimsPrincipal principal, object userContext, Dictionary<string, object> arguments, IEnumerable<string> requiredPolicies)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            var cxt = userContext as GraphQLUserContext;

            ClaimsPrincipal claimsPrincipal = _httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? _httpContextAccessor.HttpContext.User : null;
            if (claimsPrincipal == null)
            {
                var response = await _httpContextAccessor.HttpContext.AuthenticateAsync().ConfigureAwait(false);
                if (response.Succeeded)
                {
                    claimsPrincipal = response.Principal;
                    _httpContextAccessor.HttpContext.User = response.Principal;
                }
                else
                {
                    var error = response.Failure.Message;
                    var errorList = new List<string>();
                    errorList.Add(error);
                    return AuthorizationResult.Fail(errorList);
                }
            }

            var context = new AuthorizationContext();
            context.User = claimsPrincipal;
            context.UserContext = userContext;
            context.Arguments = arguments;

            var authPolicies = _authorizationSettings.GetPolicies(requiredPolicies);
            var tasks = new List<Task>();
            authPolicies.Apply(p =>
            {
                p.Requirements.Apply(r =>
                {
                    var task = r.Authorize(context);
                    tasks.Add(task);
                });
            });

            await Task.WhenAll(tasks.ToArray()).ConfigureAwait(false);
            return !context.HasErrors
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(context.Errors);
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0], CultureInfo.CurrentCulture) + s.Substring(1);
        }
    }
}
