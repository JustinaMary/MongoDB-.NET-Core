namespace PetizenApi.Options
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;

    /// <summary>
    /// The caching options for the application.
    /// </summary>
#pragma warning disable CA1710 // Identifiers should have correct suffix
    public class CacheProfileOptions : Dictionary<string, CacheProfile>
#pragma warning restore CA1710 // Identifiers should have correct suffix
    {
    }
}
