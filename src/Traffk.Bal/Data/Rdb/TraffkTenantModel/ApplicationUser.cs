using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Data.Rdb.TraffkTenantModel
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public partial class ApplicationUser
    {
        partial void PartialPreSave()
        {
            if (this.ContactId < 1)
            {
                this.Contact = new UserContact
                {
                    PrimaryEmail = this.Email,
                    FullName = this.UserName
                };
            }
        }

        public void LogSignInAttempt(SignInResult res)
            => LogActivity("SignInAttempt", res.Succeeded);

        private void LogActivity(string activityType, bool result)
        {
            var logger = Log.ForContext(typeof(EventType).Name, EventType.LoggingEventTypes.Account.ToString());
            logger.Information("Activity UserId={@Id} ContactId={@ContactId} ActivityType={@ActivityType} ActivityResult={@ActivityResult}", this.Id, this.ContactId, activityType, result);
        }
    }
}
