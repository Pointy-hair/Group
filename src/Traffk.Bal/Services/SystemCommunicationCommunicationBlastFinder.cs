using Microsoft.EntityFrameworkCore;
using RevolutionaryStuff.Core;
using System.Linq;
using System.Threading.Tasks;
using Traffk.Bal.Communications;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Bal.Services
{
    public class SystemCommunicationCommunicationBlastFinder : ICommunicationBlastFinder
    {
        private readonly TraffkTenantModelDbContext Rdb;
        private readonly ICreativeSettingsFinder CreativeSettingsFinder;

        public SystemCommunicationCommunicationBlastFinder(TraffkTenantModelDbContext rdb, ICreativeSettingsFinder creativeSettingsFinder)
        {
            Requires.NonNull(rdb, nameof(rdb));
            Rdb = rdb;
            CreativeSettingsFinder = creativeSettingsFinder;
        }

        async Task<CommunicationBlast> ICommunicationBlastFinder.FindAsync(Creative creative)
        {
            var blast = await Rdb.CommunicationBlasts.FirstOrDefaultAsync(z => z.CreativeId == creative.CreativeId);
            if (blast != null) return blast;
            var comm = await Rdb.Communications.OrderBy(z => z.CommunicationId).Take(1).SingleAsync();
            var blastSpecificCreative = new Creative(creative);
            var filler = new CommunicationsFiller(CreativeSettingsFinder, null);
            filler.Flatten(blastSpecificCreative.CreativeSettings);
            Rdb.Creatives.Add(blastSpecificCreative);
            blast = new CommunicationBlast
            {
                Communication = comm,
                Creative = blastSpecificCreative,
            };
            Rdb.CommunicationBlasts.Add(blast);
            await Rdb.SaveChangesAsync();
            return blast;
        }
    }
}
