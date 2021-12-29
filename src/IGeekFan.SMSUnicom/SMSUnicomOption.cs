namespace IGeekFan.SMSUnicom
{
    public class SMSUnicomOption
    {
        /// <summary>
        /// 登录自服务平台，进入商户信息页，【商户编码】值为cpcode
        /// </summary>
        public string Cpcode { get; set; }
        /// <summary>
        /// 登录自服务平台，进入系统设置页，【AccessKey】值为key
        /// </summary>
        public string Accesskey { get; set; }
        /// <summary>
        /// 扩展码：渠道自定义接入号的扩展码，可为空；为空时传空字符串”
        /// </summary>
        public string Excode { get; set; }
        /// <summary>
        /// 融合云信接口地址
        /// </summary>
        public string SmsUrl { get; set; }
    }
}
