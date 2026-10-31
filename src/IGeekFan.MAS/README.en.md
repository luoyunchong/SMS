**English** | [简体中文](README.md)

﻿## MAS

China Mobile's Cloud MAS platform is used to send SMS and MMS messages.

- [https://mas.10086.cn/login](https://mas.10086.cn/login)

## Nuget Packages

| Package name   | Version                                                                                                                                              | Downloads                                                      | Supported platforms                      |
| -------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------- | ------------------------- |
| `IGeekFan.MAS` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.MAS.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.MAS/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.MAS.svg) | .NET4.5.2/.NETStandard2.0 |


## Usage

### .NET Core

Install the package
```console
dotnet add package IGeekFan.MAS
```
The SMS interface supports `https` and `http`. The address below is an example; obtain the address for your account from the SDK integration support contact.
```
云mas--接口联调
HTTP	普通短信	http://112.35.1.155:1992/sms/norsubmit

云mas--接口联调
https	普通短信	https://112.35.10.201:28888/sms/submit
```
appsettings.json configuration
```json
 "MASOption": {
    "Smsurl": "http://112.35.1.155:1992", //根据服务商提供
    "EcName": "", 
    "ApId": "",
    "SecretKey": "",
    "Sign": ""
  }
```
- Smsurl Submission URL, as supplied by your provider http://112.35.1.155:1992
- EcName Enterprise customer name
- ApId API account. This is not the Cloud MAS website username. In Cloud MAS, open Management → Interface Management → New SMS Interface; the username created there is the API account.
- SecretKey API credential corresponding to the account created above
- Sign Signature code. Obtain it in Cloud MAS under Management → Interface Management → SMS Access User Management.

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

- Bulk-send response
```json
 {
  "rspcod": "success",
  "msgGroup": "0105013527000001139923",
  "success": true,
  "rspText": "数据验证通过。"
}
```


### .NET FrameWork

Install the package
```console
Install-Package IGeekFan.MAS
```

appSettings configuration in web.config
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

Call 

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

## Complete code example

- [/samples/SMS_ASPNET452/Controllers/MASController.cs](../../samples/SMS_ASPNET452/Controllers/MASController.cs)

## Customize logging in ASP.NET MVC

- [/src/IGeekFan.SMSUnicom/README.md](../../src/IGeekFan.SMSUnicom/README.md)