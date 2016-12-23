using RevolutionaryStuff.Core;

namespace Traffk.Bal.Data.Rdb
{
    public enum ApplicationTypes
    {
        Default,
        [EnumeratedStringValue(TraffkHelpers.TraffkUrn + "/portal")]
        Portal,
    }

    public partial class Application
    {
        partial void OnApplicationSettingsDeserialized()
        {
            ApplicationSettings.EnsureMemberClasses();
        }
    }
}
