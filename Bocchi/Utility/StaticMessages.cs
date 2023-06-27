namespace Bocchi.Utility;

public struct StaticMessages
{
    public static readonly string AssistantMessage = Setting.Get("ASSISTANT_MESSAGE");
    public static readonly string ConvertSayingMessage = Setting.Get("CONVERT_SAYING_MESSAGE");
    public static readonly string Saying = Setting.Get("SAYING");

    public static readonly string TrialNoticeMessage = Setting.Get("TRIAL_NOTICE_MESSAGE");
    public static readonly string TrialEndMessage = Setting.Get("TRIAL_END_MESSAGE");
    public static readonly string InvalidKeyMessage = Setting.Get("INVALID_KEY_MESSAGE");
    public static readonly string KeyUpdatedMessage = Setting.Get("KEY_UPDATED_MESSAGE");

    public static readonly string ErrorMessage = Setting.Get("ERROR_MESSAGE");
}