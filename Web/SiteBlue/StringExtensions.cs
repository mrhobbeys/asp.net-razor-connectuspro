namespace SiteBlue
{
    public static class StringExtension
    {
        public static string ReplaceForXml(this string str)
        {
            if (str == null)
                return "";

            return str.Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }
    }
}