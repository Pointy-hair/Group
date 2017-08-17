using System.Xml;

namespace Traffk.Tableau.REST.Helpers
{
    /// <summary>
    /// Writes out XML for a credential
    /// </summary>
    static class CredentialXmlHelper
    {
        /// <summary>
        /// Writes out the credential element.  Used in Workbook and Data Source publication
        /// </summary>
        public static void WriteCredential(XmlWriter xmlWriter, Traffk.Tableau.REST.Helpers.CredentialManager.Credential credential)
        {
            WriteCredential(xmlWriter, credential.Name, credential.Password, credential.IsEmbedded);
        }

        /// <summary>
        /// Writes out the credential element.  Used in Workbook and Data Source publication
        /// </summary>
        public static void WriteCredential(XmlWriter xmlWriter, string connectionUserName, string password, bool isEmbedded)
        {
            xmlWriter.WriteStartElement("connectionCredentials");
            xmlWriter.WriteAttributeString("name", connectionUserName);
            xmlWriter.WriteAttributeString("password", password);
            XmlHelper.WriteBooleanAttribute(xmlWriter, "embed", isEmbedded);
            xmlWriter.WriteEndElement();
        }
    }
}
