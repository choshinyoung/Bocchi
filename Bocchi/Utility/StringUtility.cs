using System.Text.RegularExpressions;

namespace Bocchi.Utility;

public static partial class StringUtility
{
    [GeneratedRegex("^\\s*(```(cs)?\\s*(?<block_code>.+)\\s*```)|(?<code>.+)\\s*$")]
    public static partial Regex CodeRegex();
}