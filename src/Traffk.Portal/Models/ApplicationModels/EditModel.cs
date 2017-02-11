using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;

namespace TraffkPortal.Models.ApplicationModels
{
    public class EditModel
    {
        public int ApplicationId { get; set; }

        [DisplayName("Application Type")]
        [MaxLength(80)]
        public string ApplicationType { get; set; }

        [DisplayName("Application Name")]
        [MaxLength(255)]
        public string ApplicationName { get; set; }

        [DisplayName("Email Sender Name")]
        public string EmailSenderName { get; set; }

        [DisplayName("Email Sender Address")]
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
