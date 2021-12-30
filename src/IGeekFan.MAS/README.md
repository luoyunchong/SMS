## MAS

中国移动云MAS业务平台，

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

appsettings.json配置
```json
 "MASOption": {
    "Sendurl": "http://112.35.1.155:1992", //根据服务商提供
    "EcName": "",
    "ApId": "",
    "SecretKey": "",
    "Sign": ""
  }
```
- SmsUrl 短信发送地址 //根据服务商提供
- UserName 用户名
- Password 密码

```cs
builder.Services.Configure<MASOption>(builder.Configuration.GetSection("MASOption"));

builder.Services.AddTransient<MASClient>();
```

```cs
app.MapGet("/Send", (MASClient sms, string content, string mobile) =>
{
    return sms.Send(new SendRequest() { Extno = "1069012345", Content = content, Mobile = mobile });
});
```


### .NET FrameWork

安装包
```console
Install-Package IGeekFan.MAS
```

web.config中appSettings配置
```xml
<!--短信配置开始-->
<!--集团客户名称-->
<add key="ec_name" value=""/>
<!--用户名-->
<add key="user_name" value=""/>
<!--密码-->
<add key="user_passwd" value=""/>
<!--网关签名-->
<add key="sign" value=""/>
<!--发送的提交地址 根据服务商提供-->
<add key="sendurl" value="http://112.35.1.155:1992"/>
<!--短信配置结束-->
```

调用 

```csharp
[RoutePrefix("api/home")]
public class HomeController : ApiController
{
    private readonly MASOption smsOption = new MASOption()
    {
        SmsUrl = ConfigurationManager.AppSettings["SmsUrl"].ToString(),
        UserName = ConfigurationManager.AppSettings["UserName"].ToString(),
        Password = ConfigurationManager.AppSettings["Password"].ToString()
    };

    [Route("Send")]
    [HttpGet]
    public SendReponse Send(string content, string mobile)
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.Send(new SendRequest() { Extno = "1069012345", Content = content, Mobile = mobile });
    }
}
```
