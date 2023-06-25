using Bocchi.Functions.Attributes;
using Bocchi.Utility;

namespace Bocchi.Functions.Functions;

public class CommonFunctions : FunctionContext
{
    [Function("GetDateTime", "Get current date and time, e.g. '2023년 6월 17일 토요일 오후 5:27:36'")]
    public string GetDateTime()
    {
        return DateTimeUtility.GetKstDateTime().ToString("F");
    }

    [Function("Shuffle", "Shuffle given array of integer, e.g. '{ \"numbers\": [1, 2, 3, 4, 5] } -> [ 4, 2, 5, 1, 3 ]")]
    public int[] Shuffle(int[] numbers)
    {
        var random = new Random();

        return numbers.OrderBy(i => random.Next().CompareTo(random.Next())).ToArray();
    }
}