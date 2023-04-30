using System.Xml.Linq;

namespace SiteQuery.Extensions;

public class XDocumentExtension : XDocument
{
    public static XDocument? TryParse(string s)
    {
        try
        {
            return Parse(s);
        }
        catch (Exception e)
        {
            return null;
        }
    }
}