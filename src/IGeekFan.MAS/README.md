## MAS

中国移动提供的云MAS业务平台，用于短信、彩信的发送。

- [https://mas.10086.cn/login](https://mas.10086.cn/login)

## Nuget Packages

| Package name   | Version                                                                                                                                              | Downloads                                                      | 支持                      |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------- | ------------------------- |
| `IGeekFan.MAS` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.MAS.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.MAS/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.MAS.svg) | .NET4.5.2/.NETStandard2.0 |


## 使用方式

### .NET Core

安装包
```console
dotnet add package IGeekFan.MAS
```
发送短信时，同一接口为`https`和`http`，以下仅供参考，可通过`SDK`联调的`QQ`获取您实际的地址。。
```
云mas--接口联调
HTTP	普通短信	http://112.35.1.155:1992/sms/norsubmit

云mas--接口联调
https	普通短信	https://112.35.10.201:28888/sms/submit
```
appsettings.json配置
```json
 "MASOption": {
    "Smsurl": "http://112.35.1.155:1992", //根据服务商提供
    "EcName": "", 
    "ApId": "",
    "SecretKey": "",
    "Sign": ""
  }
```
- Smsurl 发送的提交地址 //根据服务商提供 http://112.35.1.155:1992
- EcName 集团客户名称
- ApId 接口账号：注意： 此处不是MAS云网站的用户名， 登入云mas   管理→接口管理→新建短信接口（这里创建）   创建里面用户名对应接口账号
- SecretKey 对应创建的接口账号的接口密码
- Sign 签名编码。在云MAS平台『管理』→『接口管理』→『短信接入用户管理』获取。

```cs
builder.Services.Configure<MASOption>(builder.Configuration.GetSection("MASOption"));

builder.Services.AddTransient<MASClient>();
```

```cs
app.MapPost("/sendmsg", (MASClient sms, [FromBody] SendMsgRequest sendMsg) =>
{
    return sms.SendMsg(sendMsg);
});

app.MapPost("/sendtmpsubmit", (MASClient sms, [FromBody] TmpsubmitRequest sendMsg) =>
{
    return sms.SendTmpsubmit(sendMsg);
});

/// <summary>
/// 批量发送短信
/// </summary>
app.MapPost("/sendmsg-batch", (MASClient sms) =>
{
    SendMsgRequest sendMsg = new SendMsgRequest();
    var dict = new Dictionary<string, string>()
    {
        {"手机号1","您的验证码是:4813" },
        {"手机号2","您的验证码是:2314" }
    };
    string contents = JsonSerializer.Serialize(dict);
    sendMsg.Content = contents;
    return sms.SendMsgAsync(sendMsg);
}).WithDisplayName("批量发送短信");
```

- 批量发送返回值
```json
 {
  "rspcod": "success",
  "msgGroup": "0105013527000001139923",
  "success": true,
  "rspText": "数据验证通过。"
}
```


### .NET FrameWork

安装包
```console
Install-Package IGeekFan.MAS
```

web.config中appSettings配置
```xml
<!--MAS短信配置开始-->
<!--发送的提交地址 根据服务商提供-->
<add key="MAS_SmsUrl" value="http://112.35.1.155:1992"/>
<!--集团客户名称-->
<add key="MAS_EcName" value=""/>
<!--用户名-->
<add key="MAS_ApId" value=""/>
<!--密码-->
<add key="MAS_SecretKey" value=""/>
<!--网关签名-->
<add key="MAS_Sign" value=""/>
<!--MAS短信配置结束-->
```

调用 

```csharp
[RoutePrefix("api/mas")]
public class MasController : ApiController
{
    private readonly MASOption smsOption = new MASOption()
    {
        Sendurl = ConfigurationManager.AppSettings["MAS_SmsUrl"],
        EcName = ConfigurationManager.AppSettings["MAS_EcName"],
        ApId = ConfigurationManager.AppSettings["MAS_ApId"],
        SecretKey = ConfigurationManager.AppSettings["MAS_SecretKey"],
        Sign = ConfigurationManager.AppSettings["MAS_Sign"]
    };

    [Route("Send")]
    [HttpGet]
    public SendMsgResponse Send(SendMsgRequest sendRequest)
    {
        var sms = new MASClient(smsOption);
        return sms.SendMsg(sendRequest);
    }
}
```

## 完整代码参考

- [/samples/SMS_ASPNET452/Controllers/MASController.cs](../../samples/SMS_ASPNET452/Controllers/MASController.cs)

## ASP.NET MVC 日志重写

- [/src/IGeekFan.SMSUnicom/README.md](../../src/IGeekFan.SMSUnicom/README.md)