namespace Traffk.Bal.Data.Rdb
{
    public partial class Application
    {
        public static class ApplicationTypes
        {
            public const string Portal = TraffkHelpers.TraffkUrn+"/portal";
        }

        partial void OnApplicationSettingsDeserialized()
        {
            ApplicationSettings.EnsureMemberClasses();
        }
    }
}
