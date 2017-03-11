using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RevolutionaryStuff.Core.ApplicationParts;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Settings;

namespace Traffk.Bal.ReportVisuals
{
    public interface ITableauTenantFinder : ITenantFinder<string>
    {

    }

    public class TableauTenantFinder : ITableauTenantFinder
    {
        public IOptions<TenantSettings> Options { get; private set; }
        public TraffkRdbContext Rdb { get; private set; }
        public int TraffkTenantId { get; private set; }
        public string TableauTenantId
        {
            get
            {
                if (String.IsNullOrEmpty(TableauTenantId_p))
                {
                    //No access to TenantFinderServiceException
                    //throw new TenantFinderServiceException(TenantServiceExceptionCodes.TableauTenantNotFound, $"tenantId={TraffkTenantId}");
                }
                return TableauTenantId_p;
            }

        }

        private string TableauTenantId_p;

        public TableauTenantFinder(IOptions<TenantSettings> options, TraffkRdbContext rdb, ITraffkTenantFinder traffkTenantFinder)
        {
            TraffkTenantId = traffkTenantFinder.GetTenantIdAsync().Result;
            Options = options;
            Rdb = rdb;
        }

        Task<string> ITenantFinder<string>.GetTenantIdAsync()
        {
            var tenantSettingsOptions = Options.Value;
            if (tenantSettingsOptions != null)
            {
                if (!String.IsNullOrEmpty(tenantSettingsOptions.TableauTenantId))
                {
                    TableauTenantId_p = tenantSettingsOptions.TableauTenantId;
                }
            }
            else
            {
                //Rdb operation here
            }

            return Task.FromResult(TableauTenantId_p);
        }
    }
}
