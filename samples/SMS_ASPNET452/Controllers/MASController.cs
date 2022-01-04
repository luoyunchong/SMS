using System.Web.Http;
using System.Configuration;
using IGeekFan.MAS;

namespace SMS_ASPNET452.Controllers
{

    [RoutePrefix("api/mas")]
    public class MasController : ApiController
    {
        private readonly MASOption smsOption = new MASOption()
        {
            SmsUrl = ConfigurationManager.AppSettings["MAS_SmsUrl"],
            EcName = ConfigurationManager.AppSettings["MAS_EcName"],
            ApId = ConfigurationManager.AppSettings["MAS_ApId"],
            SecretKey = ConfigurationManager.AppSettings["MAS_SecretKey"],
            Sign = ConfigurationManager.AppSettings["MAS_Sign"]
        };

        [Route("Send")]
        [HttpPost]
        public SendMsgResponse Send([FromBody] SendMsgRequest sendRequest)
        {
            var sms = new MASClient(smsOption);
            return sms.SendMsg(sendRequest);
        }
    }
}
