using IGeekFan.SMS57_SMGW;
using System.IO;
using System;
using System.Web;
using System.Text;

namespace SMS_ASPNET452.Controllers
{
    /// <summary>
    /// 自定义日志
    /// </summary>
    public class ErrLog : ILogger
    {
        public static ErrLog Instance = new Lazy<ErrLog>(() => new ErrLog()).Value;

        protected ErrLog()
        {
        }

        public void LogInformation(string message)
        {
            if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/log")))
            {
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/log"));
            }
            string filename = HttpContext.Current.Server.MapPath("~/log/error" + DateTime.Now.ToString("yyyyMMdd") + ".log");
            TextWriter f = new StreamWriter(filename, true, Encoding.UTF8);
            f = TextWriter.Synchronized(f);
            f.WriteLine("LogInformation:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message);
            f.Close();
        }
    }
}
