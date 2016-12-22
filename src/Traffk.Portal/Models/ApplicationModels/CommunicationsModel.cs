using System.Collections.Generic;
using System.Linq;
using Traffk.Bal.Data.Rdb;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TraffkPortal.Models.ApplicationModels
{
    public class CommunicationsModel
    {
        public ICollection<SystemCommunication> Communications { get; set; }
        public ICollection<SystemCommunication.Definition> Definitions { get; set; }
        public ICollection<MessageTemplate> MessageTemplates { get; set; }
        public IDictionary<int, Template> TemplateLookup { get; set; }

        private bool SupportsModel(int? templateId, string modelType)
        {
            if (templateId == null) return true;
            var t = TemplateLookup[templateId.Value];
            return t.ModelType == null || t.ModelType == modelType;
        }

        public IList<SelectListItem> GetCompatibleMessageTemplates(SystemCommunication.Definition definition)
        {
            var items = new List<SelectListItem>();
            foreach (var messageTemplate in MessageTemplates)
            {
                if (SupportsModel(messageTemplate.SubjectTemplateId, definition.ModelType) &&
                    SupportsModel(messageTemplate.TextBodyTemplateId, definition.ModelType) &&
                    SupportsModel(messageTemplate.HtmlBodyTemplateId, definition.ModelType)
                    )
                {
                    items.Add(new SelectListItem { Value = $"{messageTemplate.MessageTemplateId}", Text = messageTemplate.MessageTemplateTitle });
                }
            }
            items.Sort((a,b)=>a.Text.CompareTo(b.Text));
            items.Insert(0, AspHelpers.CreatePleaseSelectListItem());
            return items;
        }

        public SystemCommunication FindOrCreateCommunication(SystemCommunication.Definition def, string communicationMedium)
        {
            var c = Communications.FirstOrDefault(z => z.CommunicationPurpose == def.CommunicationPurpose && z.CommunicationMedium == communicationMedium);
            if (c == null)
            {
                c = new SystemCommunication
                {
                     CommunicationPurpose = def.CommunicationPurpose,
                     CommunicationMedium = communicationMedium
                };
                Communications.Add(c);
            }
            return c;
        }

        public CommunicationsModel()
        { }

        public CommunicationsModel(
            ICollection<SystemCommunication> communications, 
            ICollection<SystemCommunication.Definition> definitions,
            ICollection<MessageTemplate> messageTemplates,
            IDictionary<int, Template> templateLookup)
        {
            Communications = communications;
            Definitions = definitions;
            MessageTemplates = messageTemplates;
            TemplateLookup = templateLookup;
        }
    }
}
