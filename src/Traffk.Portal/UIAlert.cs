#if false
namespace TraffkPortal
{
    public enum UIAlertTypes
    {
        Success,
        Information,
        Warning,
        Danger,
    }
    public class UIAlert
    {
        public UIAlertTypes AlertType { get; }
        public string Message { get; }

        public UIAlert(UIAlertTypes alertType, string message)
        {
            AlertType = alertType;
            Message = message;
        }
    }
}
#endif