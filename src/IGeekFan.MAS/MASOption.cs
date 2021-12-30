namespace IGeekFan.MAS
{
    public class MASOption
    {
        /// <summary>
        /// 集团客户名称
        /// </summary>
        public string EcName { get; set; }
        /// <summary>
        /// 接口账号：注意： 此处不是MAS云网站的用户名， 登入云mas   管理→接口管理→新建短信接口（这里创建）   创建里面用户名对应接口账号，用户密码对应接口密码
        /// </summary>
        public string ApId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// 网关签名
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 发送的提交地址
        /// </summary>
        public string Sendurl { get; set; }
    }
}
