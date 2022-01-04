#if NETSTANDARD2_0
#endif

namespace IGeekFan.MAS
{
    /// <summary>
    /// 1.发送普通短信 一对一或多对一
    /// </summary>
    public class SendMsgRequest
    {
        /// <summary>
        /// 多对1、1对1：收信手机号码。英文逗号分隔，每批次限5000个号码，例：“13800138000,13800138001,13800138002”。
        /// 多对多：设置为空双引号 “ ”
        /// </summary>
        public string Mobiles { get; set; }
        /// <summary>
        /// 多对1、1对1：短信内容。
        /// 多对多："{
        ///\"18840310111\":\"你好！中国！\",
        ///\"17640310111\":\"你好！世界！\",
        ///\"15340310111\":\"你好！中国移动云Mas！\"
        ///}"
        ///报文格式 号码数量小于1000个
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 扩展码。依据申请开户的服务代码匹配类型而定，如为精确匹配，此项填写空字符串（""）；如为模糊匹配，此项可填写空字符串或自定义的扩展码，注：服务代码加扩展码总长度不能超过20位。
        /// </summary>
        public string AddSerial { get; set; }
    }
}
