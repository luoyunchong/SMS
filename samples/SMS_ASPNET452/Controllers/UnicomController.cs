using IGeekFan.SMS57_SMGW;
using System.Collections.Generic;
using System.Web.Http;
using System.Configuration;
using System.Threading.Tasks;
using IGeekFan.SMSUnicom;
using System.IO;
using System.Text;
using System.Web;
using System;

namespace SMS_ASPNET452.Controllers
{
    [RoutePrefix("api/unicom")]
    public class UnicomController : ApiController
    {
        private readonly SMSUnicomOption smsOption = new SMSUnicomOption()
        {
            Cpcode = ConfigurationManager.AppSettings["Cpcode"].ToString(),
            Accesskey = ConfigurationManager.AppSettings["Accesskey"].ToString(),
            Excode = ConfigurationManager.AppSettings["Excode"].ToString(),
            SmsUrl = ConfigurationManager.AppSettings["SmsUrl"].ToString()
        };

        [Route("SendTempletMsg")]
        [HttpGet]
        public SendTempletMsgResponse SendTempletMsg(string msg, string mobiles)
        {
            var sms = new SMSUnicomClient(smsOption, UnicomErrLog.Instance);
            return sms.SendTempletMsg(new SendTempletMsgRequest() { Templetid = "1111", Msg = msg, Mobiles = mobiles });
        }

        [Route("SendTempletMsgA")]
        [HttpGet]
        public async Task<SendTempletMsgResponse> SendTempletMsgAsync(string msg, string mobiles)
        {
            var sms = new SMSUnicomClient(smsOption);
            return await sms.SendTempletMsgAsync(new SendTempletMsgRequest() { Templetid = "1111", Msg = msg, Mobiles = mobiles });
        }
    }

    public class UnicomErrLog : IGeekFan.SMSUnicom.ILogger
    {
        public static UnicomErrLog Instance = new Lazy<UnicomErrLog>(() => new UnicomErrLog()).Value;

        protected UnicomErrLog()
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
