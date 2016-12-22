using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Templates
{
    public interface ITemplateFinder
    {
        Template FindTemplateByName(string name);
        Template FindTemplateById(int id);
    }
}
