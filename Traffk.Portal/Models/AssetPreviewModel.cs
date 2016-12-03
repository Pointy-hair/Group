using Microsoft.WindowsAzure.Storage.Blob;
using RevolutionaryStuff.Core;
using System;

namespace TraffkPortal.Models
{
    public class AssetPreviewModel
    {
        public enum AssetTypes
        {
            Image,
            Javascript,
            Css,
            Other,
        }
        public Uri ContentUrl { get; set; }

        public AssetTypes AssetType { get; set; } = AssetTypes.Other;

        public string AssetKey { get; set; }

        public string DeleteScriptName { get; set; }

        public AssetPreviewModel(Uri contentUrl, AssetTypes contentType, string assetKey, string deleteScriptName=null)
        {
            ContentUrl = contentUrl;
            AssetType = contentType;
            AssetKey = assetKey;
            DeleteScriptName = deleteScriptName;
        }

        public AssetPreviewModel(CloudBlob file, string deleteScriptName = null)
        {
            ContentUrl = file.Uri;
            DeleteScriptName = deleteScriptName;
            AssetKey = file.Name.RightOf("/");
            switch (file.Properties.ContentType?.LeftOf("/").ToLower())
            {
                case "image":
                    AssetType = AssetTypes.Image;
                    break;
            }
        }
    }
}
