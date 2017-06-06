using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

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

        public EditModel(App app)
        {
            if (app == null) return;
            ApplicationId = app.AppId;
            ApplicationType = app.AppTypeStringValue;
            ApplicationName = app.AppName;
            EmailSenderAddress = app.AppSettings.EmailSenderAddress;
            EmailSenderName = app.AppSettings.EmailSenderName;
        }
    }
}
