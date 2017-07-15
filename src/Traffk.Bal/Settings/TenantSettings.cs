using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Traffk.Bal.Settings
{
    public class TenantSettings
    {
        [JsonProperty("Deerwalk")]
        public DeerwalkSettings Deerwalk { get; set; }

        [JsonProperty("Smtp")]
        public SmtpOptions Smtp { get; set; }

        [JsonProperty("EmailSenderName")]
        public string EmailSenderName { get; set; }

        [DataType(DataType.EmailAddress)]
        [JsonProperty("EmailSenderAddress")]
        public string EmailSenderAddress { get; set; }

        [JsonProperty("ProtectedHealthInformationViewableByEmailAddressHostnames")]
        public string[] ProtectedHealthInformationViewableByEmailAddressHostnames { get; set; }

        [JsonProperty("RequiresEmailAccountValidation")]
        public bool RequiresEmailAccountValidation { get; set; }

        [JsonProperty("RequiresTwoFactorAuthentication")]
        public bool RequiresTwoFactorAuthentication { get; set; }

        [JsonProperty("TableauTenantId")]
        public string TableauTenantId { get; set; }

        public class PasswordOptions
        {
            private static readonly ICollection<string> DefaultProhibitedPasswords;
            private static readonly ICollection<string> DefaultPasswordUnallowedWordList;

            private static void Populate(ICollection<string> col, Stream st)
            {
                var sr = new StreamReader(st);
                for (;;)
                {
                    var s = sr.ReadLine();
                    if (s == null) break;
                    if (s.StartsWith("#")) continue;
                    col.Add(s);
                }
            }

            static PasswordOptions()
            {
                var passwords = new HashSet<string>();
                using (var st = ResourceHelpers.GetEmbeddedResourceAsStream(TraffkHelpers.ThisAssembly, "DefaultProhibitedPasswordList.txt"))
                {
                    Populate(passwords, st);
                }
                DefaultProhibitedPasswords = passwords.AsReadOnly();
                var passwordMustNotContainWords = new HashSet<string>(Comparers.CaseInsensitiveStringComparer);
                using (var st = ResourceHelpers.GetEmbeddedResourceAsStream(TraffkHelpers.ThisAssembly, "DefaultPasswordUnallowedWordList.txt"))
                {
                    Populate(passwordMustNotContainWords, st);
                }
                DefaultPasswordUnallowedWordList = passwordMustNotContainWords.AsReadOnly();
            }

            public PasswordOptions() { }

            public PasswordOptions(PasswordOptions other) 
            {
                Copy(other);
            }

            public void Copy(PasswordOptions other)
            {
                if (other == null) return;
                RequireDigit = other.RequireDigit;
                RequiredLength = other.RequiredLength;
                RequireLowercase = other.RequireLowercase;
                RequireNonAlphanumeric = other.RequireNonAlphanumeric;
                RequireUppercase = other.RequireUppercase;
                UseDefaultProhibitedPasswordList = other.UseDefaultProhibitedPasswordList;
                ProhibitedPasswords = other.ProhibitedPasswords?.Xerox();
                UseDefaultPasswordUnallowedWordList = other.UseDefaultPasswordUnallowedWordList;
                PasswordUnallowedWordList = other.PasswordUnallowedWordList?.Xerox();
            }

            [DisplayName("Requires a Digit")]
            [JsonProperty("RequireDigit")]
            public bool RequireDigit { get; set; } = true;

            [DisplayName("Minimum Length")]
            [JsonProperty("RequiredLength")]
            public int RequiredLength { get; set; } = 8;

            [DisplayName("Requires a Lowercase Character")]
            [JsonProperty("RequireLowercase")]
            public bool RequireLowercase { get; set; } = true;

            [DisplayName("Requires Special Character")]
            [JsonProperty("RequireNonAlphanumeric")]
            public bool RequireNonAlphanumeric { get; set; } = true;

            [DisplayName("Requires an Uppercase Character")]
            [JsonProperty("RequireUppercase")]
            public bool RequireUppercase { get; set; } = true;

            [DisplayName("Use the default prohibited password list")]
            [JsonProperty("UseDefaultProhibitedPasswordList")]
            public bool UseDefaultProhibitedPasswordList { get; set; } = true;

            [JsonProperty("ProhibitedPasswords")]
            public string[] ProhibitedPasswords { get; set; }

            [JsonProperty("UseDefaultPasswordUnallowedWordList")]
            [DisplayName("Use Default Password Unallowed Term List")]
            public bool UseDefaultPasswordUnallowedWordList { get; set; } = true;

            [JsonProperty("PasswordUnallowedWordList")]
            public string[] PasswordUnallowedWordList { get; set; }

            public bool IsPasswordProhibited(string password)
            {
                return
                    password == null ||
                    (UseDefaultProhibitedPasswordList && DefaultProhibitedPasswords.Contains(password)) ||
                    (ProhibitedPasswords != null && ProhibitedPasswords.Contains(password));
            }
            public bool PasswordContainsUnallowedWord(string password)
            {
                if (PasswordUnallowedWordList != null)
                {
                    foreach (var w in PasswordUnallowedWordList)
                    {
                        if (password.Contains(w, true)) return true;
                    }
                }
                if (UseDefaultPasswordUnallowedWordList)
                {
                    foreach (var w in DefaultPasswordUnallowedWordList)
                    {
                        if (password.Contains(w, true)) return true;
                    }
                }
                return false;
            }
        }

        [JsonProperty("Password")]
        public PasswordOptions Password { get; set; }

        [JsonProperty("ReusableValues")]
        public List<ReusableValue> ReusableValues
        {
            get
            {
                if (ReusableValues_p == null)
                {
                    ReusableValues_p = new List<ReusableValue>();
                }
                return ReusableValues_p;
            }
            set { ReusableValues_p = value; }
        }
        private List<ReusableValue> ReusableValues_p;

        [JsonProperty("FiscalYearSettings")]
        public FiscalYearSettings FiscalYearSettings { get; set; }

        public TenantSettings()
        { }

        public static TenantSettings CreateFromJson(string json)
            => JsonConvert.DeserializeObject<TenantSettings>(json);

        public string ToJson()
            => JsonConvert.SerializeObject(this);

        internal void EnsureMemberClasses()
        {
            Deerwalk = Deerwalk ?? new DeerwalkSettings();
            Smtp = Smtp ?? new SmtpOptions();
            Password = Password ?? new PasswordOptions();
            FiscalYearSettings = FiscalYearSettings ?? new FiscalYearSettings();
        }
    }
}
