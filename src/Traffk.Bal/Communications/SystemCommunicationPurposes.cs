namespace Traffk.Bal.Communications
{
    public enum SystemCommunicationPurposes
    {
        [CommunicationModel(CommunicationModelTypes.CallbackUrlModel)]
        UserAcceptInvitation,
        [CommunicationModel(CommunicationModelTypes.CallbackUrlModel)]
        UserAccountVerification,
        [CommunicationModel(CommunicationModelTypes.CallbackUrlModel)]
        UserPasswordReset,
        [CommunicationModel(CommunicationModelTypes.SimpleCodeModel)]
        UserTwoFactorLoginCode,
        [CommunicationModel(CommunicationModelTypes.SimpleContentModel)]
        DirectMessage,
    }
}
