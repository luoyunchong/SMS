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
using System.Net.Security;

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
            _restClient = new RestClient(_smsOption.SmsUrl);
            _logger = logger;
        }
#else
        private readonly ILogger _logger;
        public MASClient(MASOption smsOption, ILogger logger = null)
        {
            _smsOption = smsOption;
            _restClient = new RestClient(_smsOption.SmsUrl);
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

        /// <summary>
        /// 1.发送普通短信:HTTPS客户端向云MAS平台发送短信任务请求，云MAS平台接收到请求后验证数据，验证通过将数据发送给短信网关。
        /// 2.支持：一对一或多对一模式、多对多模式
        /// 注1 解释：
        /// 多对一：发送一批短信，可以携带小于5000个号码，发送同样的一个内容。
        /// 一对一：发送一批短信，里面只包含一个号码一个短信内容。
        /// 注2 解释：多对多：发送一批短信，支持一个号码对应一个短信内容，号码数量小于1000；
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public SendMsgResponse SendMsg(SendMsgRequest msgRequest)
        {
            //参数校验序列，生成方法：将ecName、apId、secretKey、mobiles、content、sign、addSerial按序拼接（无间隔符），通过MD5（32位小写）计算得出值
            string mac = SecurityUtil.Md5(_smsOption.EcName + _smsOption.ApId + _smsOption.SecretKey + msgRequest.Mobiles + msgRequest.Content + _smsOption.Sign + msgRequest.AddSerial).ToLower();
            string resourceUrl = _smsOption.SmsUrl.StartsWith("https") ? "/sms/submit" : "/sms/norsubmit";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                secretKey = _smsOption.SecretKey,
                mobiles = msgRequest.Mobiles,
                content = msgRequest.Content,
                sign = _smsOption.Sign,
                addSerial = msgRequest.AddSerial,
                mac = mac,
            });

            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));

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

        public async Task<SendMsgResponse> SendMsgAsync(SendMsgRequest msgRequest)
        {
            //参数校验序列，生成方法：将ecName、apId、secretKey、mobiles、content、sign、addSerial按序拼接（无间隔符），通过MD5（32位小写）计算得出值
            string mac = SecurityUtil.Md5(_smsOption.EcName + _smsOption.ApId + _smsOption.SecretKey + msgRequest.Mobiles + msgRequest.Content + _smsOption.Sign + msgRequest.AddSerial).ToLower();
            string resourceUrl = _smsOption.SmsUrl.StartsWith("https") ? "/sms/submit" : "/sms/norsubmit";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                secretKey = _smsOption.SecretKey,
                mobiles = msgRequest.Mobiles,
                content = msgRequest.Content,
                sign = _smsOption.Sign,
                addSerial = msgRequest.AddSerial,
                mac = mac,
            });

            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));

            var response = await _restClient.ExecuteAsync<SendMsgResponse>(request);
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
}
