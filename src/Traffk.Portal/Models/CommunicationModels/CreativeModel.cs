using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.CommunicationModels
{
    public class CreativeModel : Creative
    {
        public int? CommunicationId { get; set; }

        [DataType(DataType.Url)]
        public IList<AssetPreviewModel> AttachmentAssets { get; set; } = new List<AssetPreviewModel>();

        public IList<IFormFile> NewAttachments { get; set; } = new List<IFormFile>();

        public CreativeModel() { }

        public CreativeModel(Creative creative, int? communicationId=null, string deleteScriptName=null)
            : base(creative)
        {
            CommunicationId = communicationId;
            foreach (var a in this.CreativeSettings.Attachments)
            {
                AttachmentAssets.Add(new AssetPreviewModel(a, deleteScriptName));
            }
        }
    }
}
