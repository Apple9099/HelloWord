using NLog;
using System;

namespace ChangePassword
{
    class MyLog
    {
        private static readonly NLog.Logger Logger_info = LogManager.GetLogger("info");
        internal static void LogInfo(string msg)
        {
            Logger_info.Info($"{DateTime.Now}: - {msg}");
        }
    }
}
