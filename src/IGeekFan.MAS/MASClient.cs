#if NETSTANDARD2_0
using IGeekFan;
using IGeekFan.MAS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif
using IGeekFan.SMS.Core;
using RestSharp;
using RestSharp.Serialization.Json;
using RestSharp.Serializers;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IGeekFan.MAS
{
    public class MASClient
    {
        private readonly MASOption _smsOption;
        private readonly RestClient _restClient;
        private static ISerializer _serializer = new Lazy<ISerializer>(() => new JsonSerializer()).Value;

#if NETSTANDARD2_0
        private readonly ILogger<MASClient> _logger;
        public MASClient(IOptionsMonitor<MASOption> smsOption, ILogger<MASClient> logger)
        {
            _smsOption = smsOption.CurrentValue;
            _restClient = new RestClient(_smsOption.Sendurl);
            _logger = logger;
        }
#else
        private readonly ILogger _logger;
        public MASClient(MASOption smsOption, ILogger logger = null)
        {
            _smsOption = smsOption;
            _restClient = new RestClient(_smsOption.Sendurl);
            if (logger == null)
            {
                _logger = NET45_Log.Instance;
            }
            else
            {
                _logger = logger;
            }
        }
#endif
        private string Md5(string s)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(s);
                string result = BitConverter.ToString(md5.ComputeHash(bytes));
                return result.Replace("-", "");
            }
        }
        
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public SendMsgResponse SendMsg(SendMsgRequest msgRequest)
        {

            /*
云mas--接口联调2 2021/9/26 10:59:01
HTTP	普通短信	http://112.35.1.155:1992/sms/norsubmit

云mas--接口联调2 2021/9/26 10:59:04
https	普通短信	https://112.35.10.201:28888/sms/submit

             */
            string addSerial = "123";
            string mac = Md5(_smsOption.EcName + _smsOption.ApId + msgRequest.Mobiles + msgRequest.Content + _smsOption.Sign + addSerial).ToLower();
            var request = new RestRequest("/sms/norsubmit", Method.POST, DataFormat.Json);
            request.AddJsonBody(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                secretKey = _smsOption.SecretKey,
                mobiles = msgRequest.Mobiles,
                content = msgRequest.Content,
                sign = _smsOption.Sign,
                addSerial = addSerial,
                mac=mac,
            });

            var response = _restClient.Execute<SendMsgResponse>(request);
            _logger.LogInformation($@"短信发送接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");
            return response.Data;
        }

    }

    public class SendMsgResponse
    {
    }

    public class SendMsgRequest
    {
        public string Mobiles { get; set; }
        public string Content { get; set; }
    }
}
