using System.Threading.Tasks;

namespace Traffk.Bal.Services
{
    public interface IVault
    {
        Task<ICredentials> GetCredentialsAsync(string credentialsKey);
        Task<string> GetSecretAsync(string uri);
    }

    public interface ICredentials
    {
        string Username { get; }
        string Password { get; }
    }
}
