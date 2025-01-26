
namespace TeslaAPIHandler.Services
{

    public interface IAuthTokenHandler
    {
        Task<string> RefreshTokenAsync(string refreshToken, string clientID);
    }

}