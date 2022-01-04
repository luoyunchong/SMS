#if NETSTANDARD2_0
#endif
using System.Collections.Generic;

namespace IGeekFan.MAS
{
    /// <summary>
    /// 2.发送普通模板短信:请求报文内容
    /// </summary>
    public class TmpsubmitRequest
    {
        /// <summary>
        /// 模板ID。在云MAS平台创建模板，路径：『短信』→『模板短信』→『模板管理』，创建后提交审核，审核通过将获得模板ID。
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 收信手机号码。英文逗号分隔，每批次限5000个号码，例：“13800138000,13800138001,13800138002”。
        /// </summary>
        public List<string> Mobiles { get; set; }
        /// <summary>
        /// 模板变量。格式：[“param1”,“param2”]，无变量模板填[""]。
        /// </summary>
        public List<string> Params { get; set; } = new List<string>();
        /// <summary>
        /// 扩展码。依据开户时申请的服务代码匹配类型而定，如为精确匹配，此项填写空字符串（""）；如为模糊匹配，此项可填写空字符串或自定义的扩展码，注：服务代码加扩展码总长度不能超过20位。
        /// </summary>
        public string AddSerial { get; set; } = "";
    }
}
