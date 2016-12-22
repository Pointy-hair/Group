using System.Linq;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Templates
{
    public class DbTemplateFinder : ITemplateFinder
    {
        private readonly TraffkRdbContext DB;
        private readonly int TenantId;

        public DbTemplateFinder(TraffkRdbContext db, int tenantId)
        {
            DB = db;
            TenantId = tenantId;
        }

        Template ITemplateFinder.FindTemplateById(int id)
        {
            return DB.Templates.FirstOrDefault(z => z.TenantId == TenantId && z.TemplateId == id);
        }

        Template ITemplateFinder.FindTemplateByName(string name)
        {
            return DB.Templates.FirstOrDefault(z => z.TenantId == TenantId && z.TemplateName == name);
        }
    }
}
