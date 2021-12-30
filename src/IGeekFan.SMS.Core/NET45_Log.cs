using System;
using System.Diagnostics;

namespace IGeekFan.SMS.Core
{
    public interface ILogger
    {
        void LogInformation(string message);
    }

    /// <summary>
    /// 适配兼容旧项目
    /// </summary>
    public class NET45_Log : ILogger
    {
        static Lazy<NET45_Log> net45Log = new Lazy<NET45_Log>(() => new NET45_Log());
        public static NET45_Log Instance => net45Log.Value;
        protected NET45_Log()
        {
        }

        public void LogInformation(string logInfo)
        {
            Trace.TraceInformation(logInfo);
        }
    }
}
