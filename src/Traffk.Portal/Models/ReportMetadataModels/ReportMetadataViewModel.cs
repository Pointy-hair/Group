using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace Traffk.Portal.Models.ReportMetadataModels
{
    public class ReportMetadataViewModel
    {
        public int ReportMetadataId { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        public ReportMetadataViewModel() { }

        public ReportMetadataViewModel(ReportMetaData reportMetadata)
        {
            ReportMetadataId = reportMetadata.ReportMetaDataId;
            Title = reportMetadata.ReportDetails.Title;
        }
    }
}
