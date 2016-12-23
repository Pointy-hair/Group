using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System.Collections.Generic;

namespace Traffk.Bal.Data.Rdb
{
    public partial class SystemCommunication
    {
        public static class CommunicationMediums
        {
            public const string Email = "email";
            public const string Sms = "sms";
            public const string Notification = "notification";

            public static readonly ICollection<string> All = new List<string> { Email, Notification, Sms }.AsReadOnly();
            public static void RequiresPredefined(string communicationMedium, string argName)
            {
                Requires.SetMembership(All, nameof(CommunicationMediums) + "." + nameof(All), communicationMedium, argName);
            }
        }

        public class Definition : IValidate
        {
            public string CommunicationPurpose { get; }
            public string ModelType { get; }
            public ICollection<string> CommunicationMediums { get; }

            public bool SupportsMedium(string communicationMedium)
            {
                return CommunicationMediums.Contains(communicationMedium);
            }

            public Definition(string communicationPurpose, string modelType, params string[] communicationMediums)
            {
                CommunicationPurpose = communicationPurpose;
                ModelType = modelType;
                CommunicationMediums = new List<string>(communicationMediums).AsReadOnly();
                Validate();
            }

            public void Validate()
            {
                CommunicationPurposes.RequiresPredefinedOrGeneralPurpose(CommunicationPurpose, nameof(CommunicationPurpose));
                Template.ModelTypes.RequiresPredefined(ModelType, nameof(ModelType));
                Requires.Positive(CommunicationMediums.Count, nameof(CommunicationMediums));
                foreach (var communicationMedium in CommunicationMediums)
                {
                    SystemCommunication.CommunicationMediums.RequiresPredefined(communicationMedium, nameof(communicationMedium));
                }
            }
        }

        public static class Definitions
        {
            public static class General
            {
                public static readonly Definition AcceptInvitation = new Definition(CommunicationPurposes.AcceptInvitation, Template.ModelTypes.CallbackUrl, CommunicationMediums.Email);
                public static readonly Definition AccountVerification = new Definition(CommunicationPurposes.AccountVerification, Template.ModelTypes.CallbackUrl, CommunicationMediums.Email);
                public static readonly Definition PasswordReset = new Definition(CommunicationPurposes.PasswordReset, Template.ModelTypes.CallbackUrl, CommunicationMediums.Email);
                public static readonly Definition TwoFactorLoginCode = new Definition(CommunicationPurposes.TwoFactorLoginCode, Template.ModelTypes.SimpleCodeModel, CommunicationMediums.Email, CommunicationMediums.Sms);

                public static readonly ICollection<Definition> All = new List<Definition> { AcceptInvitation, AccountVerification, PasswordReset, TwoFactorLoginCode }.AsReadOnly();
            }

            public static readonly ICollection<Definition> None = new List<Definition>().AsReadOnly();
            public static readonly ICollection<Definition> All = new List<Definition>(General.All).AsReadOnly();
            public static ICollection<Definition> GetByApplicationType(ApplicationTypes applicationType)
            {
                switch (applicationType)
                {
                    case ApplicationTypes.Portal:
                        return General.All;
                    default:
                        return None;
                }
            }
        }


        public static class CommunicationPurposes
        {
            private const string SystemPrefix = "System:";
            public const string AcceptInvitation = SystemPrefix + "AcceptInvitation";
            public const string AccountVerification = SystemPrefix + "AccountVerification";
            public const string PasswordReset = SystemPrefix + "PasswordReset";
            public const string TwoFactorLoginCode = SystemPrefix + "TwoFactorLoginCode";

            private const string GeneralPrefix = "General:";
            public const string DirectMessage = GeneralPrefix + "DirectMessage";

            public static readonly ICollection<string> All = new List<string> { AcceptInvitation, AccountVerification, PasswordReset, TwoFactorLoginCode }.AsReadOnly();

            public static void RequiresPredefinedOrGeneralPurpose(string communicationPurpose, string argName)
            {
                Requires.Text(communicationPurpose, argName);
                if (communicationPurpose.StartsWith(GeneralPrefix)) return;
                Requires.SetMembership(All, nameof(CommunicationPurposes) + "." + nameof(All), communicationPurpose, argName);
            }
        }
    }
}
