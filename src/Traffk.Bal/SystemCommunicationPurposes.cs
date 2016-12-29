namespace Traffk.Bal
{
    public enum SystemCommunicationPurposes
    {
        [CommunicationModel(CommunicationModelTypes.CallbackUrl)]
        UserAcceptInvitation,
        [CommunicationModel(CommunicationModelTypes.CallbackUrl)]
        UserAccountVerification,
        [CommunicationModel(CommunicationModelTypes.CallbackUrl)]
        UserPasswordReset,
        [CommunicationModel(CommunicationModelTypes.SimpleCodeModel)]
        UserTwoFactorLoginCode,
        [CommunicationModel(CommunicationModelTypes.SimpleContentModel)]
        DirectMessage,
    }
}
