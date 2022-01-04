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
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        private Dictionary<string, string> statusKeyValue = new Dictionary<string, string>()
        {
            {"IllegalMac","mac校验不通过。"},
            {"IllegalSignId","无效的签名编码。"},
            {"InvalidMessage","非法消息，请求数据解析失败"},
            {"InvalidUsrOrPwd","非法用户名/密码。"},
            {"NoSignId","非法用户名/密码。"},
            {"success","数据验证通过。"},
            {"TooManyMobiles","数据验证通过。"},
            {"InvalidMobile","手机号格式不正确" }
        };

        private string GetText(string key)
        {
            if (statusKeyValue.ContainsKey(key))
            {
                return statusKeyValue[key];
            }
            return "";
        }

        #region 1.发送普通短信
        private RestRequest GetSendMsgRequest(SendMsgRequest msgRequest)
        {
            //参数校验序列，生成方法：将ecName、apId、secretKey、mobiles、content、sign、addSerial按序拼接（无间隔符），通过MD5（32位小写）计算得出值
            string mobiles = string.Join(",", msgRequest.Mobiles);
            string mac = SecurityUtil.Md5(_smsOption.EcName + _smsOption.ApId + _smsOption.SecretKey + mobiles + msgRequest.Content + _smsOption.Sign + msgRequest.AddSerial).ToLower();
            string resourceUrl = _smsOption.SmsUrl.StartsWith("https") ? "/sms/submit" : "/sms/norsubmit";
            RestRequest request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                mobiles = mobiles,
                content = msgRequest.Content,
                sign = _smsOption.Sign,
                addSerial = msgRequest.AddSerial,
                mac = mac,
            });

            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));

            return request;
        }

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
            RestRequest request = GetSendMsgRequest(msgRequest);

            IRestResponse<SendMsgResponse> response = _restClient.Execute<SendMsgResponse>(request);
            if (response.Data != null) response.Data.RspText = GetText(response.Data.Rspcod);

            _logger.LogInformation($@"1.发送普通短信接口:
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
            RestRequest request = GetSendMsgRequest(msgRequest);
            IRestResponse<SendMsgResponse> response = await _restClient.ExecuteAsync<SendMsgResponse>(request);
            if (response.Data != null) response.Data.RspText = GetText(response.Data.Rspcod);

            _logger.LogInformation($@"短信发送接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }
        #endregion

        #region 2.发送普通模板短信
        private RestRequest GetSendTmpsubmit(TmpsubmitRequest msgRequest)
        {
            string mobiles = string.Join(",", msgRequest.Mobiles);
            string @params = _serializer.Serialize(msgRequest.Params);
            string mac = SecurityUtil.Md5(_smsOption.EcName + _smsOption.ApId + _smsOption.SecretKey + msgRequest.TemplateId + mobiles + @params + _smsOption.Sign + msgRequest.AddSerial).ToLower();
            RestRequest request = new RestRequest("/sms/tmpsubmit", Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                templateId = msgRequest.TemplateId,
                mobiles = mobiles,
                @params = msgRequest.Params,
                sign = _smsOption.Sign,
                addSerial = msgRequest.AddSerial,
                mac = mac,
            });

            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));


            return request;
        }

        /// <summary>
        /// 2.发送普通模板短信:HTTPS客户端向云MAS平台发送短信任务请求，云MAS平台接收到请求后验证数据，验证通过将数据发送给短信网关。
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public TmpsubmitResponse SendTmpsubmit(TmpsubmitRequest msgRequest)
        {
            RestRequest request = GetSendTmpsubmit(msgRequest);

            IRestResponse<TmpsubmitResponse> response = _restClient.Execute<TmpsubmitResponse>(request);
            if (response.Data != null) response.Data.RspText = GetText(response.Data.Rspcod);

            _logger.LogInformation($@"2.发送普通模板短信接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        public async Task<TmpsubmitResponse> SendTmpsubmitAsync(TmpsubmitRequest msgRequest)
        {
            RestRequest request = GetSendTmpsubmit(msgRequest);

            IRestResponse<TmpsubmitResponse> response = await _restClient.ExecuteAsync<TmpsubmitResponse>(request);
            if (response.Data != null) response.Data.RspText = GetText(response.Data.Rspcod);

            _logger.LogInformation($@"2.发送普通模板短信接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }
        #endregion


        /// <summary>
        /// 无测试环境，无法测试。3.获取状态报告:客户需要自己发起请求获取状态报告数据，云MAS返回状态报告集合数据给客户；
        /// </summary>
        /// <param name="msgRequest"></param>
        /// <returns></returns>
        public List<ReportResponse> GetReport()
        {
            RestRequest request = new RestRequest("/sms/report", Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                secretKey = _smsOption.SecretKey,
            });
            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));

            IRestResponse<List<ReportResponse>> response = _restClient.Execute<List<ReportResponse>>(request);

            _logger.LogInformation($@"3.获取状态报告接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            Content：{response.Content}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 无测试环境，无法测试。 4.获取上行短信:客户需要自己发起请求获取状态上行数据，云MAS返回状态上行短信集合数据给客户；
        /// </summary>
        /// <returns></returns>
        public List<DeliverResponse> GetDeliver()
        {
            RestRequest request = new RestRequest("/sms/deliver", Method.POST, DataFormat.Json);
            if (_smsOption.SmsUrl.StartsWith("https"))
            {
                _restClient.RemoteCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            }
            string serializaParams = _serializer.Serialize(new
            {
                ecName = _smsOption.EcName,
                apId = _smsOption.ApId,
                secretKey = _smsOption.SecretKey,
            });
            request.AddBody(Convert.ToBase64String(Encoding.UTF8.GetBytes(serializaParams)));

            IRestResponse<List<DeliverResponse>> response = _restClient.Execute<List<DeliverResponse>>(request);

            _logger.LogInformation($@"4.获取上行短信接口:
            序列化HashCode:{_serializer.GetHashCode()}
            请求地址:{response.ResponseUri}
            请求参数:{_serializer.Serialize(request.Body)},
            返回参数：{_serializer.Serialize(response.Data)}
            Content：{response.Content}
            返回ErrorMessage：{response.ErrorMessage}
            返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }
    }

    public class DeliverResponse
    {
        public string SmsContent { get; set; }
        public string Mobile { get; set; }
        public string SendTime { get; set; }
        public string AddSerial { get; set; }
    }

    public class ReportResponse
    {
        /// <summary>
        /// 发送成功状态码：DELIVRD。
        /// </summary>
        public string ReportStatus { get; set; }
        /// <summary>
        /// 回执手机号，每批次返回一个号码。
        /// </summary>
        public List<string> Mobile { get; set; }
        /// <summary>
        /// 提交时间，格式：yyyyMMddHHmmss。
        /// </summary>
        public string SubmitDate { get; set; }
        /// <summary>
        /// 接收时间，格式同上。
        /// </summary>
        public string ReceiveDate { get; set; }
        /// <summary>
        /// 发送失败的状态码，如 DB:0140。
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// 消息批次号， 唯一编码，与前文响应中的 
        /// </summary>
        public string MsgGroup { get; set; }
    }
}
