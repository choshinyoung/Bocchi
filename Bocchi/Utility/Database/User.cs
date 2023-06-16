using MongoDB.Bson;

namespace Bocchi.Utility.Database;

public class User
{
    public ObjectId Id;

    public bool IsTrial;

    public string? OpenAiKey;
    public int UsedFreeRequest;

    public ulong UserId;

    public User(ulong userId)
    {
        UserId = userId;
        IsTrial = true;
        UsedFreeRequest = 0;
        OpenAiKey = null;
    }
}