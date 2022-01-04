#if NETSTANDARD2_0
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

namespace IGeekFan.SMSUnicom
{
    public class SMSUnicomClient
    {
        private readonly SMSUnicomOption _smsOption;
        private readonly RestClient _restClient;
        private static ISerializer _serializer = new Lazy<ISerializer>(() => new JsonSerializer()).Value;

#if NETSTANDARD2_0
        private readonly ILogger<SMSUnicomClient> _logger;
        public SMSUnicomClient(IOptionsMonitor<SMSUnicomOption> smsOption, ILogger<SMSUnicomClient> logger)
        {
            _smsOption = smsOption.CurrentValue;
            _restClient = new RestClient(_smsOption.SmsUrl);
            _logger = logger;
        }
#else
        private readonly ILogger _logger;
        public SMSUnicomClient(SMSUnicomOption smsOption, ILogger logger = null)
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
        /// 2.1 短信发送接口
        ///Sign： MD5签名，签名字符串为cpcode+ msg+mobiles+excode+templetid+key，其中key为融合云信平台分配的私钥，将签名字符串用MD5加密后转换为小写字符串。
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public SendTempletMsgResponse SendTempletMsg(SendTempletMsgRequest msgRequest)
        {
            string sign = SecurityUtil.Md5(_smsOption.Cpcode + msgRequest.Msg + msgRequest.Mobiles + _smsOption.Excode + msgRequest.Templetid + _smsOption.Accesskey).ToLower();
            var request = new RestRequest("/umcinterface/sendtempletmsg", Method.POST, DataFormat.Json);
            request.AddJsonBody(new
            {
                cpcode = _smsOption.Cpcode,
                msg = msgRequest.Msg,
                mobiles = msgRequest.Mobiles,
                excode = _smsOption.Excode,
                templetid = msgRequest.Templetid,
                sign = sign
            });

            var response = _restClient.Execute<SendTempletMsgResponse>(request);
            _logger.LogInformation($@"短信发送接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");
            return response.Data;
        }

        /// <summary>
        /// 2.1 短信发送接口 异步
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public async Task<SendTempletMsgResponse> SendTempletMsgAsync(SendTempletMsgRequest msgRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            string sign = SecurityUtil.Md5(_smsOption.Cpcode + msgRequest.Msg + msgRequest.Mobiles + _smsOption.Excode + msgRequest.Templetid + _smsOption.Accesskey).ToLower();
            var request = new RestRequest("/umcinterface/sendtempletmsg", Method.POST, DataFormat.Json);
            request.AddJsonBody(new
            {
                cpcode = _smsOption.Cpcode,
                msg = msgRequest.Msg,
                mobiles = msgRequest.Mobiles,
                excode = _smsOption.Excode,
                templetid = msgRequest.Templetid,
                sign = sign
            });

            var response = await _restClient.ExecuteAsync<SendTempletMsgResponse>(request, cancellationToken);
            _logger.LogInformation($@"短信发送接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");
            return response.Data;
        }

    }
}
