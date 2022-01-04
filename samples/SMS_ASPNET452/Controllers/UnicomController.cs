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
            Cpcode = ConfigurationManager.AppSettings["Unicom_Cpcode"],
            Accesskey = ConfigurationManager.AppSettings["Unicom_Accesskey"],
            Excode = ConfigurationManager.AppSettings["Unicom_Excode"],
            SmsUrl = ConfigurationManager.AppSettings["Unicom_SmsUrl"]
        };

        [Route("SendTempletMsg")]
        [HttpGet]
        public SendTempletMsgResponse SendTempletMsg(string msg, string mobiles)
        {
            var sms = new SMSUnicomClient(smsOption, ErrLog.Instance);
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
}
