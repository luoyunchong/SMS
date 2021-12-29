namespace IGeekFan.SMSUnicom
{
    public class SendTempletMsgRequest
    {
        /// <summary>
        /// 模板参数值，如果包含多个参数，以半角英文逗号分隔
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 11位手机号，如果包含多个手机号，请用半角英文逗号分隔，最多支持300个手机号
        /// </summary>
        public string Mobiles { get; set; }
        /// <summary>
        /// 渠道自定义接入号的扩展码，可为空；为空时传空字符串””
        /// </summary>
        public string Templetid { get; set; }

    }
}
