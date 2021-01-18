using Microsoft.AspNetCore.Identity;
using System;

namespace PetizenApi.Providers
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
    public class ApplicationRole : IdentityRole<Guid>
    {
    }
}
