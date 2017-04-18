using RevolutionaryStuff.Core;

namespace Traffk.Bal.Data.Rdb
{
    public enum AppTypes
    {
        Default,
        Portal,
    }

    public partial class App
    {
        partial void OnAppSettingsDeserialized()
        {
            AppSettings.EnsureMemberClasses();
        }
    }
}
