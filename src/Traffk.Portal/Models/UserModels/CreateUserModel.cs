using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TraffkPortal.Models.UserModels
{
    public class CreeateUserModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Emails Of Each User")]
        public string Emails { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public bool SendInvitationEmail { get; set; } = true;

        public string InvitationText { get; set; }

        public IEnumerable<string> AssignedRoleIds { get; set; }
    }
}
