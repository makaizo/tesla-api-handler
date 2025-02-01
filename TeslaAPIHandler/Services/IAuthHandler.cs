using System.Threading.Tasks;

namespace TeslaAPIHandler.Services
{

    public interface IAuthTokenHandler
    {
        Task<(string accessToken, string refreshToken)> RefreshTokenAsync(string refreshToken, string clientID);
    }

}