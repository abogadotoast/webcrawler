using System.Text;

namespace WebCrawler.Utilities
{
    public static class StringUtilities
    {
        public static string JoinStringsWithAPlus(List<string> items)
        {
            StringBuilder sb = new();
            for (int i = 0; i < items.Count; i++)
            {
                sb.Append(items[i]);
                if (i < items.Count - 1)
                {
                    sb.Append(" + ");
                }
            }
            return sb.ToString();
        }
    }
}
