using System;
using System.Threading.Tasks;

namespace TeslaAPIHandler.Services.Mock
{
    public class MockAuthTokenHandler : IAuthTokenHandler
    {
        public Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, string clientID)
        {
            return Task.FromResult(("mock_access_token_12345", refreshToken));
        }
    }
}
