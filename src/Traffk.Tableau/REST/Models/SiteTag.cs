using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
/// <summary>
/// Information about a Tag
/// </summary>
public class SiteTag
{
    public readonly string Label;
    /// <summary>
    /// Any developer/diagnostic notes we want to indicate
    /// </summary>
    public readonly string DeveloperNotes;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userNode"></param>
    public SiteTag(XmlNode tagNode)
    {
        if (tagNode.Name.ToLower() != "tag")
        {
            throw new Exception("Unexpected content - not tag");
        }

        this.Label = tagNode.Attributes["label"].Value;
    }

    public override string ToString()
    {
        return "Tag: " + this.Label;
    }

}
