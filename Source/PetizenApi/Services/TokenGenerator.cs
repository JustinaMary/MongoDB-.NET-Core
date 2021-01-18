using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using PetizenApi.Database;
using PetizenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PetizenApi.Services
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _options;
        private readonly MongoConnection context = null;
        private static Random random = new Random();

        public TokenGenerator(IOptions<JwtOptions> options, IOptions<MongoSettings> settings)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _options = options.Value;
            context = new MongoConnection(settings);
        }

        public async Task<TokenResponse> GetTokenResponseAsync(TokenRequest tokenRequest)
        {
            if (tokenRequest == null) throw new ArgumentNullException(nameof(tokenRequest));
            try
            {
                var tokenResponse = new TokenResponse();
                var claims = new List<Claim>
            {
                new Claim("Email", tokenRequest.Email),
                new Claim(ClaimTypes.Sid, tokenRequest.UserId.ToString()),
                new Claim("UserRole",tokenRequest.Roles.ToString())
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_options.ExpiryMinutes));

                var token = new JwtSecurityToken(
                    _options.Issuer,
                    _options.Issuer,
                    claims,
                    expires: expires,
                    signingCredentials: creds
                );

                var AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
                //create RefresToken
                var refreshToken = new RefreshToken();
                refreshToken.Username = tokenRequest.Email;
                refreshToken.IpAddress = tokenRequest.IpAddress;
                refreshToken.Token = RandomRefreshToken(40);
                refreshToken.ExpiresOn = DateTime.Now.AddDays(Convert.ToDouble(_options.RefreshExpiryDay));
                //insert refresh token
                await InsUpdRefreshTokenAsync(refreshToken).ConfigureAwait(false);
                tokenResponse.AccessToken = AccessToken;
                tokenResponse.RefreshToken = refreshToken.Token;
                tokenResponse.Expires = expires;

                return tokenResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<TokenResponse> GetFromRefreshTokenAsync(string RefreshToken)
        {
            try
            {
                var refreshToken = new RefreshToken();
                var tokenResponse = new TokenResponse();

                refreshToken = GetRefreshToken(RefreshToken, "");
                //token got cancelled
                if (refreshToken.Revoked)
                {
                    refreshToken = new RefreshToken();
                    return tokenResponse;
                }

                //check for refreshtoken expiry
                if (refreshToken.ExpiresOn < DateTime.Now)
                {
                    //update refresh token
                    refreshToken.Token = RandomRefreshToken(40);
                    refreshToken.ExpiresOn = DateTime.Now.AddDays(Convert.ToDouble(_options.RefreshExpiryDay));

                    await InsUpdRefreshTokenAsync(refreshToken).ConfigureAwait(false);
                }

                if (!string.IsNullOrEmpty(refreshToken.Username))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, refreshToken.Username),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
            };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_options.ExpiryMinutes));

                    var token = new JwtSecurityToken(
                        _options.Issuer,
                        _options.Issuer,
                        claims,
                        expires: expires,
                        signingCredentials: creds
                    );

                    string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    tokenResponse.AccessToken = AccessToken;
                    tokenResponse.RefreshToken = refreshToken.Token;
                    tokenResponse.Expires = expires;
                }


                return tokenResponse;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public async Task<IEnumerable<RefreshToken>> GetAllRefreshTokenAsync()
        {
            return await context.RefreshToken.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public RefreshToken GetRefreshToken(string RefreshToken, string id)
        {
            var builder = Builders<RefreshToken>.Filter;
            var filterDefine = FilterDefinition<RefreshToken>.Empty;

            if (!string.IsNullOrEmpty(id))
            {
                filterDefine = builder.Eq(d => d.RefreshId, id);

            }
            else if (!string.IsNullOrEmpty(RefreshToken))
            {
                filterDefine = builder.Eq(d => d.Token, RefreshToken);

            }

            return context.RefreshToken
                                  .Find(filterDefine)
                                  .FirstOrDefault();
        }

        public async Task InsUpdRefreshTokenAsync(RefreshToken item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            try
            {
                var tokenUpdate = Builders<RefreshToken>.Update.
                    Set(x => x.Username, item.Username).
                    Set(x => x.Token, item.Token).
                    Set(x => x.ExpiresOn, item.ExpiresOn).
                    Set(x => x.IpAddress, item.IpAddress).
                    Set(x => x.Revoked, item.Revoked);

                //will update if data is already present if not insert the record
                await context.RefreshToken.UpdateOneAsync(t => t.Username == item.Username && t.IpAddress == item.IpAddress, tokenUpdate, new UpdateOptions { IsUpsert = true }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
                throw;
            }
        }

        public static string RandomRefreshToken(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
#pragma warning disable SCS0005 // Weak random generator
              .Select(s => s[random.Next(s.Length)]).ToArray());
#pragma warning restore SCS0005 // Weak random generator

        }
    }
}
