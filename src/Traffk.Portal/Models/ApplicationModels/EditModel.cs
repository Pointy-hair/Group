using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.ApplicationModels
{
    public class EditModel
    {
        public int ApplicationId { get; set; }

        [MaxLength(80)]
        public string ApplicationType { get; set; }

        [MaxLength(255)]
        public string ApplicationName { get; set; }

        public string EmailSenderName { get; set; }

        [EmailAddress]
        public string EmailSenderAddress { get; set; }

        public EditModel()
        { }

        public EditModel(Application app)
        {
            if (app == null) return;
            ApplicationId = app.ApplicationId;
            ApplicationType = app.ApplicationTypeStringValue;
            ApplicationName = app.ApplicationName;
            EmailSenderAddress = app.ApplicationSettings.EmailSenderAddress;
            EmailSenderName = app.ApplicationSettings.EmailSenderName;
        }
    }
}
