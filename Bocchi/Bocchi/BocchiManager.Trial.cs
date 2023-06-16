using Bocchi.Utility.Database;

namespace Bocchi;

public static partial class BocchiManager
{
    public const int TrialCount = 10;

    public static (bool isTrial, bool isAvailable) CheckTrialCount(ulong userId)
    {
        var user = DbManager.GetUser(userId);

        return (user.IsTrial, user.UsedFreeRequest < TrialCount);
    }

    public static int GetTrialCount(ulong userId)
    {
        var user = DbManager.GetUser(userId);

        return user.UsedFreeRequest;
    }

    public static int UpdateTrialCount(ulong userId)
    {
        var user = DbManager.GetUser(userId);

        if (user is { IsTrial: true, UsedFreeRequest: < TrialCount })
        {
            user.UsedFreeRequest++;

            DbManager.SetUser(user.UserId, u => u.UsedFreeRequest, user.UsedFreeRequest);
        }

        return user.UsedFreeRequest;
    }
}