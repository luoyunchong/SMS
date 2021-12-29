#if NETSTANDARD2_0
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
#endif

namespace IGeekFan.SMS57_SMGW
{
    public class SMGW_Option
    {
        /// <summary>
        /// 短信发送地址
        /// </summary>
        public string SmsUrl { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
