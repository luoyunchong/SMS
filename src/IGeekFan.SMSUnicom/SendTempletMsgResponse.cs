namespace IGeekFan.SMSUnicom
{
    public class SendTempletMsgResponse
    {
        /// <summary>
        /// 内部订单号，匹配状态报告使用
        /// </summary>
        public string Taskid { get; set; }
        /// <summary>
        /// 响应码
        /// </summary>
        public string Resultcode { get; set; }
        /// <summary>
        /// 错误码说明，请参考下节《错误码说明》
        /// </summary>
        public string Resultmsg { get; set; }

    }
}
