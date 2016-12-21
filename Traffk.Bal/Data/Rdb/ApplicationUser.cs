﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Traffk.Bal.Settings;

namespace Traffk.Bal.Data.Rdb
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public partial class ApplicationUser : IdentityUser, ITraffkTenanted
    {
        public override string ToString() => $"{base.ToString()} name=[{this.UserName}], tenantId={this.TenantId}";

        [Column("TenantId")]
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        [JsonIgnore]
        [IgnoreDataMember]
        public Tenant Tenant { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAtUtc { get; set; }

        [NotMapped]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [DataType("json")]
        [Column("UserSettings")]
        public string UserSettingsJson { get; set; }
    }

    public partial class ApplicationUser : IPreSave
    {
        [NotMapped]
        public UserSettings Settings
        {
            get
            {
                if (Settings_p == null)
                {
                    Settings_p = UserSettings.CreateFromJson(UserSettingsJson) ?? new UserSettings();
                }
                return Settings_p;
            }
            set { Settings_p = value; }
        }
        private UserSettings Settings_p;

        void IPreSave.PreSave()
        {
            if (Settings_p != null)
            {
                var json = Settings_p.ToJson();
                if (UserSettingsJson != json)
                {
                    UserSettingsJson = json;
                };
            }
        }

        public void LogSignInAttempt(TraffkRdbContext rdb, SignInResult res)
        {
            rdb.UserActivities.Add(new UserActivity
            {
                UserId = this.Id,
                ActivityType = "SignInAttempt",
                ActivityResult = res.Succeeded ? "Success" : "Failure"
            });
        }
    }
}