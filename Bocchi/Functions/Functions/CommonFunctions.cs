﻿using Bocchi.Functions.Attributes;
using Bocchi.Utility;

namespace Bocchi.Functions.Functions;

public class CommonFunctions : FunctionModuleBase<FunctionContext>
{
    [Function("GetDateTime", "Get current date and time, e.g. '2023년 6월 17일 토요일 오후 5:27:36'")]
    public Task<string> GetDateTime()
    {
        return Task.FromResult(DateTimeUtility.GetKstDateTime().ToString("F"));
    }
}