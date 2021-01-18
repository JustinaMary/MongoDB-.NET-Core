using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace PetizenApi.Models
{
    public class RefreshToken
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RefreshId { get; set; }

        public string Username { get; set; }

        public string Token { get; set; }

        public DateTime ExpiresOn { get; set; }

        public string IpAddress { get; set; }

        public bool Revoked { get; set; }

        public DateTime InsertedDate { get; set; } = DateTime.Now;
    }

    public class TokenRequest
    {
        public string Email { get; set; }
        public Guid UserId { get; set; }

        private readonly List<string> items = new List<string>();
        public List<string> Roles { get { return items; } }
        public string IpAddress { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime Expires { get; set; }
    }

    public class JwtOptions
    {
        public string SecretKey { get; set; }

        public int ExpiryMinutes { get; set; }

        public string Issuer { get; set; }

        public int RefreshExpiryDay { get; set; }
    }
}
