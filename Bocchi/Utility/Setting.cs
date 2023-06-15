using Microsoft.Extensions.Configuration;

namespace Bocchi.Utility;

public static class Setting
{
    private static readonly IConfigurationRoot Root;

    static Setting()
    {
        Root = new ConfigurationBuilder().AddJsonFile("Data/setting.json").Build();
    }

    public static string Get(string key)
    {
        return Root.GetSection(key).Value!;
    }

    public static T Get<T>(string key) where T : IParsable<T>
    {
        return T.Parse(Root.GetSection(key).Value!, null);
    }

    public static bool GetBool(string key)
    {
        return Root.GetSection(key).Value == "True";
    }

    public static List<T> GetList<T>(string key)
    {
        return Root.GetSection(key).Get<List<T>>()!;
    }
}