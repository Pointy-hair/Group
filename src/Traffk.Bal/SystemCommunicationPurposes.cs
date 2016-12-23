namespace Traffk.Bal
{
    public enum SystemCommunicationPurposes
    {
        [CommunicationModel(CommunicationModels.CallbackUrl)]
        UserAcceptInvitation,
        [CommunicationModel(CommunicationModels.CallbackUrl)]
        UserAccountVerification,
        [CommunicationModel(CommunicationModels.CallbackUrl)]
        UserPasswordReset,
        [CommunicationModel(CommunicationModels.SimpleCodeModel)]
        UserTwoFactorLoginCode,
        [CommunicationModel(CommunicationModels.SimpleContentModel)]
        DirectMessage,
    }
}
