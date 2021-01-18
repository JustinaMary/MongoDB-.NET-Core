namespace PetizenApi.Options
{
    using GraphQL.Server;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using PetizenApi.Models;
    using PetizenApi.Providers;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// All options for the application.
    /// </summary>
    public class ApplicationOptions
    {
        public ApplicationOptions() => this.CacheProfiles = new CacheProfileOptions();

        [Required]
        public CacheProfileOptions CacheProfiles { get; }

        public CompressionOptions Compression { get; set; }

        [Required]
        public ForwardedHeadersOptions ForwardedHeaders { get; set; }

        [Required]
        public GraphQLOptions GraphQL { get; set; }

        [Required]
        public KestrelServerOptions Kestrel { get; set; }

        [Required]
        public EmailConfiguration EmailConfiguration { get; set; }

        [Required]
        public ApplicationUrl ApplicationUrl { get; set; }

        [Required]
        public JwtOptions JwtToken { get; set; }
    }
}
