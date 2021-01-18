using PetizenApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetizenApi.Services
{
    public interface ITokenGenerator
    {
        Task<TokenResponse> GetTokenResponseAsync(TokenRequest tokenRequest);

        Task<TokenResponse> GetFromRefreshTokenAsync(string RefreshToken);

        Task<IEnumerable<RefreshToken>> GetAllRefreshTokenAsync();

        RefreshToken GetRefreshToken(string RefreshToken, string id);

        Task InsUpdRefreshTokenAsync(RefreshToken item);

    }
}
