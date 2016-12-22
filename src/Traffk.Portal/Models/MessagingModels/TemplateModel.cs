using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.MessagingModels
{
    public class TemplateModel : Template
    {
        public enum TemplateModelEditors
        {
            SingleLineText,
            MultiLineText,
            Html,
        }

        public string TemplatePartName { get; set; }

        public TemplateModelEditors Editor { get; set; }

        public TemplateModel() { }

        public TemplateModel(Template template, TemplateModelEditors editor= TemplateModelEditors.Html, string partName = "Reusable")
            : base(template)
        {
            TemplatePartName = partName;
            Editor = editor;
        }
    }
}
