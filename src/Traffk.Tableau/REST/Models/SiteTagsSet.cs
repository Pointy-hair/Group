using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// Information about Tags associated with content in a site
/// </summary>
public class SiteTagsSet
{
    private readonly IReadOnlyList<SiteTag> tags;
    public IEnumerable<SiteTag> Tags => tags;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userNode"></param>
    public SiteTagsSet(XmlNode tagsNode, string xmlNamespace)
    {
        if (tagsNode.Name.ToLower() != "tags")
        {
            throw new Exception("Unexpected content - not tags");
        }

        //Build a set of tags
        var tags = new List<SiteTag>();
        //Get the project tags
        var tagsSet = tagsNode.SelectNodes("tag", xmlNamespace);

        foreach(var tagNode in tagsSet)
        {
            var newTag = new SiteTag((XmlNode) tagNode);
            tags.Add(newTag);

        }
        this.tags = tags.AsReadOnly();
    }

    /// <summary>
    /// String representation
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "tags: " + this.TagSetText;
    }


    /// <summary>
    /// Text of the tag set
    /// </summary>
    public string TagSetText
    {
        get
        {
            if (tags == null) return "";

            var sb = new StringBuilder();
            int numItems = 0;
            foreach (var tag in tags)
            {
                if (numItems > 0)
                {
                    sb.Append(" ");
                }
                sb.Append(tag.Label);
                numItems++;
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// True of the specified tag can be found in the set
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool IsTaggedWith(string tag)
    {
        var tagSet = tags;
        //No tags?
        if(tagSet == null)
        {
            return false;
        }

        //Look for hte tag
        foreach(var thisTag in tagSet)
        {
            if(thisTag.Label == tag)
            {
                return true;
            }
        }

        return false;
    }
}
