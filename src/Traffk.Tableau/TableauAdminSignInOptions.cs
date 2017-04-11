
namespace Traffk.Tableau
{
    public class TableauAdminSignInOptions : ITableauUserCredentials
    {
        public TableauAdminSignInOptions()
        { }

        public string Username { get; set; }
        public string Password { get; set; }

        string ITableauUserCredentials.UserName => Username;
        string ITableauUserCredentials.Password => Password;
    }
}
