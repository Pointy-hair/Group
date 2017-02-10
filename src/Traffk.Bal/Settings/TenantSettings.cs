using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Traffk.Bal.Settings
{
    [DataContract]
    public class TenantSettings
    {
        private static readonly DataContractJsonSerializer JsonSerializer = new DataContractJsonSerializer(typeof(TenantSettings));

        [DataMember(Name = "Deerwalk")]
        public DeerwalkSettings Deerwalk { get; set; }

        [DataMember(Name = "Smtp")]
        public SmtpOptions Smtp { get; set; }

        [DataMember(Name = "EmailSenderName")]
        public string EmailSenderName { get; set; }

        [DataType(DataType.EmailAddress)]
        [DataMember(Name = "EmailSenderAddress")]
        public string EmailSenderAddress { get; set; }

        [DataMember(Name = "ProtectedHealthInformationViewableByEmailAddressHostnames")]
        public string[] ProtectedHealthInformationViewableByEmailAddressHostnames { get; set; }

        [DataMember(Name = "RequiresEmailAccountValidation")]
        public bool RequiresEmailAccountValidation { get; set; }

        [DataMember(Name = "RequiresTwoFactorAuthentication")]
        public bool RequiresTwoFactorAuthentication { get; set; }

        [DataContract]
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
                using (var st = Stuff.GetEmbeddedResourceAsStream(TraffkHelpers.ThisAssembly, "DefaultProhibitedPasswordList.txt"))
                {
                    Populate(passwords, st);
                }
                DefaultProhibitedPasswords = passwords.AsReadOnly();
                var passwordMustNotContainWords = new HashSet<string>(Comparers.CaseInsensitiveStringComparer);
                using (var st = Stuff.GetEmbeddedResourceAsStream(TraffkHelpers.ThisAssembly, "DefaultPasswordUnallowedWordList.txt"))
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
            [DataMember(Name = "RequireDigit")]
            public bool RequireDigit { get; set; } = true;

            [DisplayName("Minimum Length")]
            [DataMember(Name = "RequiredLength")]
            public int RequiredLength { get; set; } = 8;

            [DisplayName("Requires a Lowercase Character")]
            [DataMember(Name = "RequireLowercase")]
            public bool RequireLowercase { get; set; } = true;

            [DisplayName("Requires Special Character")]
            [DataMember(Name = "RequireNonAlphanumeric")]
            public bool RequireNonAlphanumeric { get; set; } = true;

            [DisplayName("Requires an Uppercase Character")]
            [DataMember(Name = "RequireUppercase")]
            public bool RequireUppercase { get; set; } = true;

            [DisplayName("Use the default prohibited password list")]
            [DataMember(Name = "UseDefaultProhibitedPasswordList")]
            public bool UseDefaultProhibitedPasswordList { get; set; } = true;

            [DataMember(Name = "ProhibitedPasswords")]
            public string[] ProhibitedPasswords { get; set; }

            [DataMember(Name = "UseDefaultPasswordUnallowedWordList")]
            [DisplayName("Use Default Password Unallowed Term List")]
            public bool UseDefaultPasswordUnallowedWordList { get; set; } = true;

            [DataMember(Name = "PasswordUnallowedWordList")]
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

        [DataMember(Name = "Password")]
        public PasswordOptions Password { get; set; }

        [DataMember(Name = "ReusableValues")]
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

        public TenantSettings()
        { }

        public static TenantSettings CreateFromJson(string json)
        {
            return JsonSerializer.ReadObjectFromString<TenantSettings>(json);
        }

        public string ToJson()
        {
            return JsonSerializer.WriteObjectToString(this);
        }

        internal void EnsureMemberClasses()
        {
            Deerwalk = Deerwalk ?? new DeerwalkSettings();
            Smtp = Smtp ?? new SmtpOptions();
            Password = Password ?? new PasswordOptions();
        }
    }
}
