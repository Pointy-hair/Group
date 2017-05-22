using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;
using Traffk.Tableau.REST.Models;

namespace Traffk.Tableau.REST.Helpers
{
    /// <summary>
    /// Re-Maps all of the references to published datasources inside a Workbook to point to a different server/site.
    /// This transformation is needed to successfully copy a Workbook from one site/server to another site/server.
    /// </summary>
    public static class TableauFileEditor
    {
        public static void UpdateDatasourceDatabaseName(string pathToDatasourceFile, string newDatabasename, string completedFileFolderPath)
        {
            var dataSourceName = Path.GetFileNameWithoutExtension(pathToDatasourceFile) + @".tds";
            var tdsFolderPath = completedFileFolderPath;
            var tdsFolder = Directory.CreateDirectory(tdsFolderPath);
            var tempFolderPath = Path.GetDirectoryName(pathToDatasourceFile) + @"\Temp";
            var tempFolder = Directory.CreateDirectory(tempFolderPath); //Checks if folder already exists

            var fileExtension = Path.GetExtension(pathToDatasourceFile).ToLower();
            if ((fileExtension != ".tds") && (fileExtension != ".tdsx"))
            {
                //irrelevant file
                return;
            }

            if (fileExtension == ".tdsx")
            {
                //Change to ZIP
                var zipFilePath = Path.ChangeExtension(pathToDatasourceFile, ".zip");
                File.Move(pathToDatasourceFile, zipFilePath);

                ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath);
            }

            if (fileExtension == ".tds")
            {
                File.Move(pathToDatasourceFile, tempFolderPath);
            }

            foreach (var file in Directory.GetFiles(tempFolderPath)) //should only be a single file here
            {
                var stream = new FileStream(file, FileMode.Open);
                var xDoc = XDocument.Load(stream);
                stream.Dispose();
                var connectionElements = xDoc.Descendants(XName.Get("connection")).Where(x => x.Attribute(XName.Get("dbname")) != null);
                foreach (var element in connectionElements)
                {
                    element.Attributes(XName.Get("dbname")).SingleOrDefault().Value = newDatabasename;
                }
                var newStream = new FileStream(file, FileMode.Create);
                xDoc.Save(newStream);
                newStream.Dispose();

                File.Move(file, tdsFolderPath + @"\" + dataSourceName);
            }
        }

        public static void UpdateWorkbookFileSiteReferences(string pathToWorkbookFile, SiteInfo site)
        {
            var siteName = site.Name;
            var fileExtension = Path.GetExtension(pathToWorkbookFile).ToLower();
            if (fileExtension != ".twb")
            {
                //irrelevant file
                return;
            }

            var stream = new FileStream(pathToWorkbookFile, FileMode.Open);
            var xDoc = XDocument.Load(stream);
            stream.Dispose();

            //should be separate method for DRY
            var workbookElement =
                xDoc.Descendants(XName.Get("workbook")).Descendants(XName.Get("repository-location")).First();
            workbookElement.Attributes(XName.Get("path")).SingleOrDefault().Value = "/t/" + siteName + "/workbooks";
            workbookElement.Attributes(XName.Get("site")).SingleOrDefault().Value = siteName;

            var datasourceElement = xDoc.Descendants(XName.Get("datasource")).Descendants(XName.Get("repository-location")).SingleOrDefault();
            datasourceElement.Attributes(XName.Get("path")).SingleOrDefault().Value = "/t/" + siteName + "/datasources";
            datasourceElement.Attributes(XName.Get("site")).SingleOrDefault().Value = siteName;

            var newStream = new FileStream(pathToWorkbookFile, FileMode.Create);
            xDoc.Save(newStream);
            newStream.Dispose();
        }
    }
}
