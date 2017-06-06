namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
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
