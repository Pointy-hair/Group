using RevolutionaryStuff.Core.Collections;
using RevolutionaryStuff.PowerBiToys.Objects;
using System.Collections.Generic;

namespace TraffkPortal.Models.ReportingModels
{
    public class ResourcesModel
    {
        public TreeNode<PowerBiResource> Root { get; set; }
        public IList<PowerBiResource> Items { get; set; }
    }
}
