using System;
using System.Xml;

namespace Traffk.Tableau.REST.Helpers
{
    partial class CredentialManager
    {
        /// <summary>
        /// Parse the credential and add it to the credential manager
        /// </summary>
        private static void helper_parseCredentialNode(Traffk.Tableau.REST.Helpers.CredentialManager credentialManager, XmlNode credentialNode)
        {
            var contentType =  XmlHelper.ReadTextAttribute(credentialNode, "contentType", "");
            var contentProjectName = XmlHelper.ReadTextAttribute(credentialNode, "contentProjectName");
            var contentName = XmlHelper.ReadTextAttribute(credentialNode, "contentName"); 

            var dbUserName = XmlHelper.ReadTextAttribute(credentialNode, "dbUser");
            var dbPassword = XmlHelper.ReadTextAttribute(credentialNode, "dbPassword");
            var isEmbedded = XmlHelper.ReadBooleanAttribute(credentialNode, "credentialIsEmbedded", false);

            //Sanity checking
            if(string.IsNullOrWhiteSpace(contentName))
            {
                throw new Exception("Credential is missing content name");
            }

            if(contentType == "workbook")
            {
                credentialManager.AddWorkbookCredential(contentName, contentProjectName, dbUserName, dbPassword, isEmbedded);
            }
            else if(contentType == "datasource")
            {
                credentialManager.AddDatasourceCredential(contentName, contentProjectName, dbUserName, dbPassword, isEmbedded);
            }
            else
            {
                throw new Exception("Unknown credential content type: " + contentType);
            }
        }
    }
}
