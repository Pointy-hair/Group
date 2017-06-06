using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;

namespace TraffkPortal.Models.UserModels
{
    public class UserModel
    {
        public string Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public IEnumerable<string> AssignedRoleIds { get; set; }

        public IEnumerable<string> AssignedRoleNames { get; set; }

        [Display(Name = "Roles")]
        public string AssignedRoleNamesCsv => AssignedRoleNames.ToCsv();

        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool? CanAccessProtectedHealthInformation { get; set; }

        public UserModel()
        { }

        public UserModel(ApplicationUser user, IDictionary<string, ApplicationRole> rolesById, bool? canAccessProtectedHealthInformation=null)
            : this()
        {
            Id = user.Id;
            CreatedAtUtc = user.CreatedAtUtc;
            UserName = user.UserName;
            Email = user.Email;
            EmailConfirmed = user.EmailConfirmed;
            PhoneNumber = user.PhoneNumber;
            PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            TwoFactorEnabled = user.TwoFactorEnabled;
            LockoutEnabled = user.LockoutEnabled;
            AccessFailedCount = user.AccessFailedCount;
            LockoutEnd = user.LockoutEnd;
            AssignedRoleIds = user.Roles.ConvertAll(r => r.RoleId);
            AssignedRoleNames = user.Roles.Map(rolesById, r => r.RoleId).ConvertAll(r=>r.Name);
            CanAccessProtectedHealthInformation = canAccessProtectedHealthInformation;
        }
    }
}
