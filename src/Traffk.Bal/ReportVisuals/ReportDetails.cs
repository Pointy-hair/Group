using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Traffk.Bal.Settings;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public string PreviewImageUrl { get; set; }
        public ICollection<string> Tags { get; set; }
        public bool ContainsPhi { get; set; }
        public bool Shared { get; set; }
        public bool Favorite { get; set; }
        public ICollection<KeyValuePair<string, string>> Parameters { get; set; }
        public bool CanExport { get; set; }
        public VisualContext VisualContext { get; set; }
        public DateTime LastEdit { get; set; }
        public string LastEditedField { get; set; }

        public void Merge(IReportMetaData reportMetaData)
        {
            //Could be useful if merging in recent changes from parent RMD
        }

        public static ReportDetails CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<ReportDetails>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
