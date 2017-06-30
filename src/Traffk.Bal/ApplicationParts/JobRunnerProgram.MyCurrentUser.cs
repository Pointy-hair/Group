using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Bal.Services;

namespace Traffk.Bal.ApplicationParts
{
    public partial class JobRunnerProgram
    {
        public class MyCurrentUser : ICurrentUser
        {
            public ApplicationUser User { get; set; }
        }
    }
}
