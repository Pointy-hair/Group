using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Threading;
using RevolutionaryStuff.Core;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Identity
{
    public class TraffkUserStore : UserStore<ApplicationUser, ApplicationRole, TraffkTenantModelDbContext, string>, IUserStore<ApplicationUser>
    {
        protected readonly ITraffkTenantFinder TenantFinder;

        public TraffkUserStore(ITraffkTenantFinder tenantFinder, TraffkTenantModelDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
            Requires.NonNull(tenantFinder, nameof(tenantFinder));
            TenantFinder = tenantFinder;
        }

        public override async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tenantId = await TenantFinder.GetTenantIdAsync();
            return await Context.Users.Include(u=>u.Contact).FirstOrDefaultAsync(u =>
            (u.Id == userId) &&
            (u.TenantId == tenantId),
            cancellationToken);
        }

        public override async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tenantId = await TenantFinder.GetTenantIdAsync();
            return await Context.Users.Include(u => u.Contact).FirstOrDefaultAsync(u =>
            (u.NormalizedEmail == normalizedEmail) &&
            (u.TenantId == tenantId),
            cancellationToken);
        }

        public override Task<ApplicationUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
//            return base.FindByLoginAsync(loginProvider, providerKey, cancellationToken);
        }

        public override async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tenantId = await TenantFinder.GetTenantIdAsync();
            return await Context.Users.Include(u => u.Contact).FirstOrDefaultAsync(u => 
            (u.NormalizedUserName == normalizedUserName) && 
            (u.TenantId == tenantId), 
            cancellationToken);
        }
    }
}
