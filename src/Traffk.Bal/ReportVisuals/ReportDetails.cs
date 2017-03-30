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
        public RenderingAttributeFlags? RenderingAttributes { get; set; }

        [Flags]
        public enum RenderingAttributeFlags
        {
            Vertical = 0x1,
            //B = 0x2,
            //C = 0x4,
            //D = 0x8,
            //E = 0x10,
            //F = 0x20,
            //G = 0x40,
        }

        public static ReportDetails CreateFromJson(string json)
        {
            return TraffkHelpers.JsonConvertDeserializeObjectOrFallback<ReportDetails>(json);
        }

        public string ToJson() => JsonConvert.SerializeObject(this);
    }
}
