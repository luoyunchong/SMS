using IGeekFan.SMS57_SMGW;
using System.Collections.Generic;
using System.Web.Http;
using System.Configuration;

namespace SMS_ASPNET452.Controllers
{
    [RoutePrefix("api/home")]
    public class HomeController : ApiController
    {
        private readonly SMGW_Option smsOption = new SMGW_Option()
        {
            SmsUrl = ConfigurationManager.AppSettings["SmsUrl"].ToString(),
            UserName = ConfigurationManager.AppSettings["UserName"].ToString(),
            Password = ConfigurationManager.AppSettings["Password"].ToString()
        };

        [Route("ok")]
        [HttpGet]
        public string ok()
        {
            return "ok";
        }

        [Route("Send")]
        [HttpGet]
        public SendReponse Send(string content, string mobile)
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.Send(new SendRequest() { Extno = "1069012345", Content = content, Mobile = mobile });
}

        [Route("P2P")]
        [HttpPost]
        public P2PResponse P2P(List<P2PMessage> mobileContentList)
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.P2P(new P2PRequest() { Extno = "1069012345", mobileContentList = mobileContentList }); ;
        }

        [Route("Balance")]
        [HttpGet]
        public BalanceResponse Balance()
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.Balance(new BaseRequest() { });
        }

        [Route("Report")]
        [HttpGet]
        public ReportResponse Report()
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.Report(new ReportRequest() { });
        }

        [Route("Mo")]
        [HttpGet]
        public ReportResponse Mo()
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.Mo(new ReportRequest() { });
        }

        [Route("Statis")]
        [HttpPost]
        public StatisResponse Statis([FromBody] StatisRequest req)
        {
            var sms = new SMSMailGateway(smsOption);
            return sms.Statis(req);
        }
    }
}
