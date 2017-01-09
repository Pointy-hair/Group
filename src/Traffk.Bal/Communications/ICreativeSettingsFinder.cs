using Traffk.Bal.Settings;

namespace Traffk.Bal.Communications
{
    public interface ICreativeSettingsFinder
    {
        CreativeSettings FindSettingsByName(string name);
        CreativeSettings FindSettingsById(int id);
    }
}
