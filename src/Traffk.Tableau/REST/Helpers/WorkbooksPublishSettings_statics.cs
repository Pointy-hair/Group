using System;
using System.IO;
using System.Xml;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.Helpers
{
    internal partial class WorkbookPublishSettings
    {
        private const string TableauXmlNamespaceName = "iwsOnline";
        private const string TableauXmlNamespaceUrl = "http://tableau.com/api";
        private const string XmlElement_WorkbookInfo = "WorkbookInfo";
        private const string XmlElement_ShowTabsInWorkbook = "ShowTabsInWorkbook";
        public const string XmlElement_ContentOwner = "OwnerName";
        public const string WorkbookSettingsSuffix = ".info.xml";

        public const string UnknownOwnerName = "**Unknown Owner**"; 

        /// <summary>
        /// TRUE if the file is an internal settings file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static bool IsSettingsFile(string filePath)
        {
            return filePath.EndsWith(WorkbookSettingsSuffix, StringComparison.CurrentCultureIgnoreCase);
        }


        /// <summary>
        /// Looks 
        /// </summary>
        /// <param name="ownerId">GUID of content owner</param>
        /// <param name="userLookups">set we are looking the owner up in</param>
        /// <returns></returns>
        internal static string helper_LookUpOwnerId(string ownerId, KeyedLookup<SiteUser> userLookups)
        {
            ////Sanity test
            //AppDiagnostics.Assert(!string.IsNullOrWhiteSpace(ownerId), "blank owner id to look up?");

            string contentOwnerName;
            var contentOwner = userLookups.FindItem(ownerId);
            //This should only ever happen if there is a race condition between getting the list of users, and downloading the content, where a user did not exist when we asked for the list of users
            //Should be very rare
            if (contentOwner == null)
            {
                return UnknownOwnerName;
            }
            return contentOwnerName = contentOwner.Name;
        }

        /// <summary>
        /// Look up any saved settings we have associated with a workbook on our local file systemm
        /// </summary>
        /// <param name="workbookWithPath"></param>
        /// <returns></returns>
        internal static  WorkbookPublishSettings GetSettingsForSavedWorkbook(string workbookWithPath)
        {

            //Find the path to the settings file
            var pathToSettingsFile = PathForSettingsFile(workbookWithPath);
            if(!File.Exists(pathToSettingsFile))
            {
                return GenerateDefaultSettings();
            }

            //===================================================================
            //We've got a setings file, let's parse it!
            //===================================================================
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new FileStream(pathToSettingsFile, FileMode.Open));

            //Show sheets
            bool showSheetsInTabs = ParseXml_GetShowSheetsAsTabs(xmlDoc);
            string ownerName = ParseXml_GetOwnerName(xmlDoc);

            //Return the Settings data
            return new WorkbookPublishSettings(showSheetsInTabs, ownerName); 
        }

        /// <summary>
        /// Looks for the ShowTabs information inside the XML document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns>
        /// TRUE/FALSE - whether the workbook wants to show tabs in the viz
        /// </returns>
        static bool ParseXml_GetShowSheetsAsTabs(XmlDocument xmlDoc)
        {
            var xNodeShowTabs = xmlDoc.SelectSingleNode("//" + XmlElement_ShowTabsInWorkbook, TableauXmlNamespaceName);

            //If there is no node, then return the default
            if(xNodeShowTabs == null)
            {
                return false;
            }

            return XmlHelper.SafeParseXmlAttribute_Bool(xNodeShowTabs, XmlHelper.XmlAttribute_Value, false);
        }

        /// <summary>
        /// Looks for the Owner Name information inside the XML document
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns></returns>
        internal static string ParseXml_GetOwnerName(XmlDocument xmlDoc)
        {
            var xNodeOwner = xmlDoc.SelectSingleNode("//" + XmlElement_ContentOwner, TableauXmlNamespaceName);

            //If there is no node, then return the default
            if (xNodeOwner == null)
            {
                return null;
            }

            return XmlHelper.SafeParseXmlAttribute(xNodeOwner, XmlHelper.XmlAttribute_Value, null);
        }

        /// <summary>
        /// Default settings to use if no settings file is present
        /// </summary>
        internal static WorkbookPublishSettings GenerateDefaultSettings()
        {
            return new WorkbookPublishSettings(false, null);
        }

        /// <summary>
        /// Generates the path/filename of the Settings file that corresponds to the workbook path
        /// </summary>
        /// <param name="workbookPath"></param>
        /// <returns></returns>
        internal static string PathForSettingsFile(string workbookPath)
        {
            //Sanity test
            if(string.IsNullOrWhiteSpace(workbookPath))
            {
                throw new ArgumentNullException("missing path");
            }

            //Tag an extra extension to the file
            return workbookPath + WorkbookSettingsSuffix;
        }
    }
}
