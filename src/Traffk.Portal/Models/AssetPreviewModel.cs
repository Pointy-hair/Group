using Microsoft.WindowsAzure.Storage.Blob;
using RevolutionaryStuff.Core;
using System;
using Traffk.Bal.Services;

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

        public string ContextKey { get; set; }

        public string AssetKey { get; set; }

        public string DeleteScriptName { get; set; }

        public AssetPreviewModel(Uri contentUrl, AssetTypes contentType, string assetKey, string contextKey, string deleteScriptName=null)
        {
            ContentUrl = contentUrl;
            AssetType = contentType;
            AssetKey = assetKey;
            ContextKey = contextKey;
            DeleteScriptName = deleteScriptName;
        }

        private static AssetTypes ContentTypeToAssetType(string contentType)
        {
            contentType = StringHelpers.TrimOrNull(contentType);
            if (contentType != null)
            {
                contentType = contentType.ToLower();
                switch (contentType.LeftOf("/"))
                {
                    case "image":
                        return AssetTypes.Image;
                }
            }
            return AssetTypes.Other;
        }

        public AssetPreviewModel(CloudFilePointer pointer, string deleteScriptName = null)
        {
            ContentUrl = pointer.Uri;
            AssetType = ContentTypeToAssetType(pointer.ContentType);
            AssetKey = pointer.Path;
            DeleteScriptName = deleteScriptName;
        }

        public AssetPreviewModel(CloudBlob file, string deleteScriptName = null)
        {
            ContentUrl = file.Uri;
            DeleteScriptName = deleteScriptName;
            AssetKey = file.Name.RightOf("/");
            AssetType = ContentTypeToAssetType(file.Properties.ContentType);
        }
    }
}
