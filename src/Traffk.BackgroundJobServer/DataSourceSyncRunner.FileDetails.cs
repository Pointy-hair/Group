using Renci.SshNet.Sftp;
using RevolutionaryStuff.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Traffk.BackgroundJobServer
{
    public partial class DataSourceSyncRunner
    {
        public class FileDetails
        {
            public readonly string Path;
            public readonly string FullName;
            public readonly string Name;
            public readonly long Size;
            public readonly DateTime? LastModifiedAtUtc;
            public readonly string ETag;
            public readonly byte[] ContentMD5;
            public readonly IList<string> Urns = new List<string>();

            private static string ToUnixPath(string windowsPath)
                => windowsPath.Replace('\\', '/');

            private static string ToWindowsPath(string unixPath)
                => unixPath.Replace('/', '\\');

            public FileDetails(FileDetails baseDetails, string name = null)
            {
                FullName = ToWindowsPath(baseDetails.FullName);
                Name = name ?? System.IO.Path.GetFileName(FullName);
                Path = System.IO.Path.GetDirectoryName(FullName);

                FullName = ToUnixPath(FullName);
                Path = ToUnixPath(Path);
            }

            public FileDetails(SftpFile f)
            {
                FullName = f.FullName;
                Name = f.Name;
                Path = ToUnixPath(System.IO.Path.GetDirectoryName(ToWindowsPath(FullName)));
                Size = f.Length;
                LastModifiedAtUtc = f.LastWriteTimeUtc;
            }

            public FileDetails(HttpResponseMessage f)
            {
                var h = f.Content.Headers;
                Size = h.ContentLength.GetValueOrDefault();
                if (h.ContentMD5 != null && h.ContentMD5.Length.IsBetween(1,20))
                {
                    ContentMD5 = h.ContentMD5;
                }
                ETag = f.Headers.ETag?.Tag;
                if (h.LastModified.HasValue)
                {
                    LastModifiedAtUtc = h.LastModified.Value.UtcDateTime;
                }
                Name = h.ContentDisposition?.FileName;
                if (Name == null)
                {
                    Name = System.IO.Path.GetFileName(f.RequestMessage.RequestUri.LocalPath);
                }
                Path = ToUnixPath(System.IO.Path.GetDirectoryName(f.RequestMessage.RequestUri.LocalPath));
                FullName = Path + "/" + Name;
            }
        }
    }
}
