using Microsoft.Extensions.Configuration;
using RevolutionaryStuff.Core;

namespace TraffkPortal.Services
{
    public sealed class TraffkPortalSecrets
    {
        public string PowerBiUsername { get; }
        public string PowerBiPassword { get; }

        public TraffkPortalSecrets(IConfiguration configuration)
        {
            PowerBiUsername = configuration["PowerBiUsername"];
            PowerBiPassword = configuration["PowerBiPassword"];
            
            Requires.Text(PowerBiUsername, nameof(PowerBiUsername));
            Requires.Text(PowerBiPassword, nameof(PowerBiPassword));
        }
    }
}
