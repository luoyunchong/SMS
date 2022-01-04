#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif
using IGeekFan.SMS.Core;
using RestSharp;
using RestSharp.Serialization.Json;
using RestSharp.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;

namespace IGeekFan.SMS57_SMGW
{

    public class SMSGatewayClient
    {
        #region 构造函数
        private readonly SMGW_Option _smsOption;
        private readonly RestClient _restClient;
        private static ISerializer _serializer = new Lazy<ISerializer>(() => new JsonSerializer()).Value;
#if NETSTANDARD2_0
        private readonly ILogger<SMSGatewayClient> _logger;
        public SMSGatewayClient(IOptionsMonitor<SMGW_Option> smsOption, ILogger<SMSGatewayClient> logger)
        {
            _smsOption = smsOption.CurrentValue;
            _restClient = new RestClient(_smsOption.SmsUrl);
            _logger = logger;
        }
#else
        private readonly ILogger _logger;
        public SMSGatewayClient(SMGW_Option smsOption, ILogger logger = null)
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
        private Dictionary<int, string> statusKeyValue = new Dictionary<int, string>()
        {
            {1,"消息包格式错误"},
            {2,"IP鉴权错误"},
            {3,"账号密码不正确"},
            {4,"版本号错误"},
            {5,"其它错误"},
            {6,"接入点错误（如账户本身开的是CMPP接入）"},
            {7,"账号状态异常（账号已停用）"},
            {8,"号码不能为空"},
            {9,"内容不能为空"},
            {21,"连接过多"},
            {30,"endTime<begin或 endTime-beginTime>31结束时间不能小于开始时间统计间隔不能大于31天（在statis下，建议不大于31天）"},
            {31,"时间禁止（在statis下，过于频繁，建议一个小时一次）"},
            {100 ,"系统内部错误，请联系管理员"},
            {102,"单次提交的号码数过多（建议200以内）"},
            {0  ,"鉴权成功"}
        };

        private string GetText(int key)
        {
            if (statusKeyValue.ContainsKey(key))
            {
                return statusKeyValue[key];
            }
            return "";
        }


        #endregion

        /// <summary>
        /// 1.	短信发送接口:用户客户端向网关提交短信
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public SendReponse Send(SendRequest sendMessage)
        {
            //http://host:port/sms?action=send&account=账号&password=密码&mobile=15100000000,15100000001&content=内容&extno=1069012345&rt=json
            string resourceUrl = $"/api/sm57/sms?action=send&account={_smsOption.UserName}&password={_smsOption.Password}&mobile={sendMessage.Mobile}&content={sendMessage.Content}&extno={sendMessage.Extno}&rt={sendMessage.rt}";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);

            var response = _restClient.Execute<SendReponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"短信发送接口:
请求地址:{response.ResponseUri}
请求参数:Mobile:{sendMessage.Mobile},
请求参数:Content:{sendMessage.Content},
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 2.	点对点发送:用于客户端向网关提交点对点短信，即一个号码一个内容的短信
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public P2PResponse P2P(P2PRequest sendMessage)
        {
            string mobileContentListStr = String.Empty;
            foreach (var mobileItem in sendMessage.mobileContentList)
            {
                mobileContentListStr += mobileItem.Mobile + "#" + mobileItem.Content + "\n";
            }
            //http://host:port/sms?action=p2p&rt=json&account=922011&password=123456&mobileContentList=13800000001%23%e4%b8%8b%e5%8f%91%e5%86%85%e5%ae%b91%0d13800000002%23%e4%b8%8b%e5%8f%91%e5%86%85%e5%ae%b92%0d13800000003%23%e4%b8%8b%e5%8f%91%e5%86%85%e5%ae%b93%0d13800000004%23%e4%b8%8b%e5%8f%91%e5%86%85%e5%ae%b94%0d&extno=10690231221
            string resourceUrl = $"/api/sm57/sms?action=p2p&account={_smsOption.UserName}&password={_smsOption.Password}&mobileContentList={HttpUtility.UrlEncode(mobileContentListStr)}&extno={sendMessage.Extno}&rt={sendMessage.rt}";

            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);
            var response = _restClient.Execute<P2PResponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"点对点发送:
请求地址:{response.ResponseUri}
请求参数:mobileContentListStr:{mobileContentListStr},
请求参数:Extno:{sendMessage.Extno},
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 3.	余额查询接口：用于客户端查询当前账户余额
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public BalanceResponse Balance(BaseRequest sendMessage)
        {
            //http://host:port/sms?action=balance&account=账号&password=密码&rt=json
            string resourceUrl = $"/api/sm57/sms?action=balance&account={_smsOption.UserName}&password={_smsOption.Password}&rt={sendMessage.rt}";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);

            var response = _restClient.Execute<BalanceResponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"余额查询接口:
请求地址:{response.ResponseUri}
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 4.	客户端主动获取状态报告接口:用于客户端到网关获取短信发送的状态报告
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public ReportResponse Report(ReportRequest sendMessage)
        {
            //http://host:port/sms?action=report&account=账号&password=密码&size=10&rt=json
            string resourceUrl = $"/api/sm57/sms?action=report&account={_smsOption.UserName}&password={_smsOption.Password}&size={sendMessage.Size}&rt={sendMessage.rt}";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);

            var response = _restClient.Execute<ReportResponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"客户端主动获取状态报告接口:
请求地址:{response.ResponseUri}
请求参数:Size:{sendMessage.Size},
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 5.	客户端主动获取手机上行接口:用户客户端向网关获取手机上行回复的短信
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public ReportResponse Mo(ReportRequest sendMessage)
        {
            //http://host:port/sms?action=report&action=report&account=账号&password=密码&size=10&rt=json
            string resourceUrl = $"/api/sm57/sms?action=report&account={_smsOption.UserName}&password={_smsOption.Password}&size={sendMessage.Size}&rt={sendMessage.rt}";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);

            var response = _restClient.Execute<ReportResponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"客户端主动获取手机上行接口:
请求地址:{response.ResponseUri}
请求参数:Size:{sendMessage.Size},
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }

        /// <summary>
        /// 7.	获取统计信息接口:用户客户端向网关获取统计信息
        /// </summary>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public StatisResponse Statis(StatisRequest sendMessage)
        {
            ///http://host:port/sms?action=statis&account=账号&password=密码&beginTime=20210224&endTime=20210224&rt=json
            string resourceUrl = $"/api/sm57/sms?action=statis&account={_smsOption.UserName}&password={_smsOption.Password}&beginTime={sendMessage.BeginTime.ToString("yyyyMMdd")}&endTime={sendMessage.EndTime.ToString("yyyyMMdd")}&rt={sendMessage.rt}";
            var request = new RestRequest(resourceUrl, Method.POST, DataFormat.Json);

            var response = _restClient.Execute<StatisResponse>(request);
            if (response.Data != null) response.Data.StatusText = GetText(response.Data.Status);

            _logger.LogInformation($@"获取统计信息接口:
请求地址:{response.ResponseUri}
请求参数:BeginTime:{sendMessage.BeginTime.ToString("yyyyMMdd")},
请求参数:EndTime:{sendMessage.EndTime.ToString("yyyyMMdd")},
请求参数:rt:{sendMessage.rt},
返回参数：{_serializer.Serialize(response.Data)}
返回ErrorMessage：{response.ErrorMessage}
返回ErrorException：{ response.ErrorException?.StackTrace}");

            return response.Data;
        }
    }

    public class P2PRequest : BaseRequest
    {
        /// <summary>
        /// 号码短信内容列表:号码内容格式为：Mobile#Content
        /// 多个号码之间用换行分割如：
        ///13800000001#下发内容1
        ///13800000002#下发内容2
        /// </summary>
        public List<P2PMessage> mobileContentList { get; set; }
        /// <summary>
        /// 接入号，即SP服务号（106XXXXXX）
        /// </summary>
        public string Extno { get; set; }
    }

    public class P2PMessage
    {
        public string Mobile { get; set; }
        public string Content { get; set; }
    }

    public class P2PResponse : BaseResponse
    {
        public List<P2PResponseList> List { get; set; }
    }

    public class P2PResponseList
    {
        public string Mid { get; set; }
        public string Mobile { get; set; }
        public int Result { get; set; }
    }


    public class BaseResponse
    {
        /// <summary>
        /// 请求结果，具体参见STATUS错误代码表
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 当前账户余额，单位厘
        /// </summary>
        public int Balance { get; set; }
        /// <summary>
        /// STATUS错误代码表
        /// </summary>
        public string StatusText { get; set; }
    }

    public class StatisResponse : BaseResponse
    {
        public List<StatisResponseList> List { get; set; }
    }

    public class StatisResponseList
    {
        public string StatisTIme { get; set; }
        public int Total { get; set; }
        public int Success { get; set; }
        public int Unknown { get; set; }
        public int Fail { get; set; }
        public float SuccessRate { get; set; }
    }

    public class StatisRequest : BaseRequest
    {
        /// <summary>
        /// 开始时间:格式：yyyymmdd，例：20210224
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间 注：开始时间与结束时间不能大于31天
        /// </summary>
        public DateTime EndTime { get; set; }

    }

    public class ReportRequest : BaseRequest
    {
        /// <summary>
        /// 获取报告的数量:默认1000，最小10，最大10000
        /// </summary>
        public int Size { get; set; } = 1000;
    }

    public class ReportResponse : BaseResponse
    {
        public List<ReportResponseList> list { get; set; }
    }

    public class ReportResponseList
    {
        public int Flag { get; set; }
        public string Mid { get; set; }
        public string Spid { get; set; }
        public string AccessCode { get; set; }
        public string Mobile { get; set; }
        public string Stat { get; set; }
        public string Time { get; set; }
    }

    /// <summary>
    /// 余额查询 响应数据
    /// </summary>
    public class BalanceResponse : BaseResponse
    {
        /// <summary>
        /// POSTCHARGE=后付费 PRECHARGE=预付费  
        /// </summary>
        public string ChargeType { get; set; }
    }

    public class BaseRequest
    {
        /// <summary>
        /// 响应数据类型:json
        /// </summary>
        [DefaultValue("json")]
        public string rt { get; set; } = "json";
    }

    public class SendReponse : BaseResponse
    {
        public List<SendReponseList> List { get; set; }
    }

    public class SendReponseList
    {
        public string Mid { get; set; }
        public string Mobile { get; set; }
        public int Result { get; set; }
    }


    public class SendRequest : BaseRequest
    {
        /// <summary>
        /// 全部被叫号码:发信发送的目的号码.多个号码之间用半角逗号隔开,最多500个号码 
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 发送内容:短信的内容，内容需要UTF-8 URLEncode编码
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 接入号，即SP服务号（106XXXXXX）
        /// </summary>
        public string Extno { get; set; }
    }
}
