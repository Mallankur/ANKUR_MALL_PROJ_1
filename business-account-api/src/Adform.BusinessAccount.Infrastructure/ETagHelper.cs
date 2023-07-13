namespace Adform.BusinessAccount.Infrastructure;

public static class ETagHelper
{
    public static long FromWeakETagHeader(string timestamp)
    {
        if (string.IsNullOrEmpty(timestamp))
            return 0;

        var startOffset = 0;
        var endOffset = 0;

        if (timestamp.StartsWith("W/\""))
            startOffset += 3;
        else if (timestamp.StartsWith("\""))
            startOffset += 1;

        if (timestamp.EndsWith("\""))
            endOffset += 1;

        var ts = timestamp.Substring(startOffset, timestamp.Length - startOffset - endOffset);

        return string.IsNullOrEmpty(ts) ? 0 : long.Parse(ts);
    }

}