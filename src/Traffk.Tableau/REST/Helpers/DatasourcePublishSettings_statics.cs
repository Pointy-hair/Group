using System;
using System.IO;
using System.Xml;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.Helpers
{
    internal partial class DatasourcePublishSettings
    {
        private const string XmlElement_DatasourceInfo = "DatasourceInfo";
        private const string DatasourceSettingsSuffix = WorkbookPublishSettings.WorkbookSettingsSuffix;

        /// <summary>
        /// TRUE if the file is an internal settings file
        /// </summary>
        internal static bool IsSettingsFile(string filePath)
        {
            return filePath.EndsWith(DatasourceSettingsSuffix, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Save Datasource metadata in a XML file along-side the workbook file
        /// </summary>
        internal static void CreateSettingsFile(SiteDatasource ds, string localDatasourcePath, KeyedLookup<SiteUser> userLookups)
        {

            string contentOwnerName = null; //Start off assuming we have no content owner information
            if (userLookups != null)
            {
                contentOwnerName = WorkbookPublishSettings.helper_LookUpOwnerId(ds.OwnerId, userLookups);
            }

            var xml = System.Xml.XmlWriter.Create(new FileStream(PathForSettingsFile(localDatasourcePath), FileMode.Open));
            xml.WriteStartDocument();
            xml.WriteStartElement(XmlElement_DatasourceInfo);

            //If we have an owner name, write it out
            if (!string.IsNullOrWhiteSpace(contentOwnerName))
            {
                XmlHelper.WriteValueElement(xml,  WorkbookPublishSettings.XmlElement_ContentOwner, contentOwnerName);
            }
            xml.WriteEndElement(); //end: WorkbookInfo
            xml.WriteEndDocument();
            xml.Dispose();
        }

        /// <summary>
        /// Generates the path/filename of the Settings file that corresponds to the datasource path
        /// </summary>
        private static string PathForSettingsFile(string datasourcePath)
        {
            return WorkbookPublishSettings.PathForSettingsFile(datasourcePath);
        }


        /// <summary>
        /// Look up any saved settings we have associated with a datasource on our local file systemm
        /// </summary>
        /// <param name="datasourceWithPath"></param>
        /// <returns></returns>
        internal static Traffk.Tableau.REST.Helpers.DatasourcePublishSettings GetSettingsForSavedDatasource(string datasourceWithPath)
        {
            //Sanity test: If the datasource is not there, then we probably have an incorrect path
            //AppDiagnostics.Assert(File.Exists(datasourceWithPath), "Underlying datasource does not exist");

            //Find the path to the settings file
            var pathToSettingsFile = PathForSettingsFile(datasourceWithPath);
            if (!File.Exists(pathToSettingsFile))
            {
                return new Traffk.Tableau.REST.Helpers.DatasourcePublishSettings(null);
            }

            //We've got a setings file, let's parse it!
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(new FileStream(pathToSettingsFile, FileMode.Open));

            //Show sheets
            string ownerName = WorkbookPublishSettings.ParseXml_GetOwnerName(xmlDoc);

            //Return the Settings data
            return new Traffk.Tableau.REST.Helpers.DatasourcePublishSettings(ownerName);
        }

    }
}
