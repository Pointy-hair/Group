
using System;
using Microsoft.Extensions.Options;

namespace Traffk.Tableau
{
    public class TableauUserCredentials : ITableauUserCredentials
    {
        //Need public setters for DI
        public string Username { get; set; }
        public string Password { get; set; }

        public TableauUserCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public TableauUserCredentials()
        {
            
        }

        string ITableauUserCredentials.UserName => Username;
        string ITableauUserCredentials.Password => Password;
    }

    public class TableauAdminCredentials : TableauUserCredentials, ITableauUserCredentials
    {
        public TableauAdminCredentials()
        {
            
        }

        string ITableauUserCredentials.UserName => Username;
        string ITableauUserCredentials.Password => Password;
    }

    //Placeholder
    public class MyTableauAdminCredentials : ITableauUserCredentials
    {
        private readonly TableauAdminCredentials Options;

        public MyTableauAdminCredentials(IOptions<TableauAdminCredentials> options)
        {
            Options = options.Value;
        }

        string ITableauUserCredentials.UserName => Options.Username;

        string ITableauUserCredentials.Password => Options.Password;
    }
}
