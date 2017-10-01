using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Traffk.Utility;

namespace Traffk.Bal.ReportVisuals
{
    public class ReportVisual : ReportResource, IReportVisual
    {
        [JsonProperty("Id")]
        public long Id { get; set; }

        [JsonIgnore]
        long IReportVisual.Id
        {
            get => Id;
            set => Id = value;
        }

        [JsonProperty("Title")]
        string IReportVisual.Title
        {
            get => Title;
            set => Title = value;
        }

        [JsonProperty("Description")]
        string IReportVisual.Description
        {
            get => Description;
            set => Description = value;
        }

        [JsonProperty("ExternalReportKey")]
        string IReportVisual.ExternalReportKey { get; set; }

        [JsonProperty("FolderPath")]
        public string FolderPath { get; set; }

        [JsonIgnore]
        string IReportVisual.FolderPath
        {
            get => FolderPath;
            set => FolderPath = value;
        }

        [JsonProperty("PreviewImageUrl")]
        string IReportVisual.PreviewImageUrl { get; set; }

        [JsonProperty("Tags")]
        [JsonConverter(typeof(ConcreteTypeConverter<List<string>>))]
        ICollection<string> IReportVisual.Tags { get; set; } = new List<string>();
        
        [JsonProperty("ContainsPhi")]
        bool IReportVisual.ContainsPhi { get; set; }

        [JsonProperty("ParentId")]
        public int? ParentId { get; set; }

        [JsonIgnore]
        int? IReportVisual.ParentId
        {
            get => ParentId;
            set => ParentId = value;
        }

        [JsonProperty("OwnerContactId")]
        long? IReportVisual.OwnerContactId { get; set; }

        [JsonProperty("Shared")]
        bool IReportVisual.Shared { get; set; }

        [JsonProperty("Favorite")]
        bool IReportVisual.Favorite { get; set; }

        [JsonProperty("Parameters")]
        ICollection<KeyValuePair<string, string>> IReportVisual.Parameters { get; set; } =
            new List<KeyValuePair<string, string>>();

        [JsonProperty("CanExport")]
        bool IReportVisual.CanExport { get; set; }

        [JsonProperty("LastEdit")]
        DateTime IReportVisual.LastEdit { get; set; }

        [JsonProperty("LastEditedField")]
        string IReportVisual.LastEditedField { get; set; }

        [JsonProperty("VisualContext")]
        VisualContext IReportVisual.VisualContext { get; set; }

        [JsonProperty("RenderingAttributes")]
        ReportDetails.RenderingAttributeFlags? IReportVisual.RenderingAttributes { get; set; }

        [JsonProperty("FolderName")]
        public string FolderName
        {
            get
            {
                var regex = new Regex(@"^\/(?<folderName>[^\\]+)$");
                var folderNameMatch = regex.Match(FolderPath);
                var folderName = folderNameMatch.Groups["folderName"].Value;
                if (String.IsNullOrEmpty(folderName))
                {
                    folderName = "Traffk Reports";
                }
                return folderName;
            }

        }
    }
}
