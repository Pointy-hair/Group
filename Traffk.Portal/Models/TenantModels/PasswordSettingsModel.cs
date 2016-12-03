using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.Linq;
using Traffk.Bal;
using Traffk.Bal.Settings;

namespace TraffkPortal.Models.TenantModels
{
    public class PasswordSettingsModel : TenantSettings.PasswordOptions
    {
        public string ProhibitedPasswordsTextArea
        {
            get { return this.ProhibitedPasswords.Join(" "); }
            set { this.ProhibitedPasswords = value.ToArrayFromHumanDelineatedString(true); }
        }

        public string PasswordUnallowedWordListTextArea
        {
            get { return this.PasswordUnallowedWordList.Join(" "); }
            set { this.PasswordUnallowedWordList = value.ToArrayFromHumanDelineatedString(true); }
        }

        public PasswordSettingsModel()
        { }

        public PasswordSettingsModel(TenantSettings.PasswordOptions p)
            : base(p)
        { }
    }
}
