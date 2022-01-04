namespace IGeekFan.MAS
{
    public class SendMsgResponse
    {
        /// <summary>
        /// 响应状态，详见下表
        /// </summary>
        public string Rspcod { get; set; }
        /// <summary>
        /// 消息批次号，由云MAS平台生成，用于关联短信发送请求与状态报告，注：若数据验证不通过，该参数值为空。
        /// </summary>
        public string MsgGroup { get; set; }
        /// <summary>
        /// 数据校验结果。
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 根据响应状态 解析出具体的文本
        /// </summary>
        public string RspText { get; set; }
    }
}
