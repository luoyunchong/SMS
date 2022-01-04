### SMS 企信通短信客户接口说明文档

- (SMGW) 代指 短信网关
- (SMGW) 即：SMS Gateway

```
下发地址：http(s)://ip:port/api/sm57/sms?action=send
点对点地址：http://host:port/sms?action=p2p
余额地址：http(s)://ip:port/api/sm57/sms?action=balance
报告地址：http(s)://ip:port/api/sm57/sms?action=report
上行地址：http(s)://ip:port/api/sm57/sms?action=mo
统计地址：http://host:port/api/sm57/sms?action=statis
```

具体参考 [SMS57-SMGW-客户接口说明文档](../../docs/SMS57-SMGW-客户接口说明文档.docx)

好像是同求宝[http://tqiu.cn/](http://tqiu.cn/) 这个网站提供短信接口，因为是实施发的文档。

## Nuget Packages

| Package name| Version| Downloads|支持                                                                         |
|  ------------ |  ------------ |  ------------ | ------------|
| `IGeekFan.SMS57_SMGW` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.SMS57_SMGW.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.SMS57_SMGW/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.SMS57_SMGW.svg) |.NET4.5.2/.NETStandard2.0


## 使用方式

### .NET Core

安装包
```console
dotnet add package IGeekFan.SMS57_SMGW
```

appsettings.json配置
```json
 "SMGW_Option": {
    "SmsUrl": "http://116.236.156.232:8010", //根据服务商提供
    "UserName": "",
    "Password": ""
  }
```
- SmsUrl 短信发送地址 //根据服务商提供
- UserName 用户名
- Password 密码

```csharp
builder.Services.Configure<SMGW_Option>(builder.Configuration.GetSection("SMGW_Option"));

builder.Services.AddTransient<SMSGatewayClient>();
```

```csharp

app.MapGet("/Send", (SMSGatewayClient sms, string content, string mobile) =>
{
    return sms.Send(new SendRequest() { Extno = "1069012345", Content = content, Mobile = mobile });
});
app.MapPost("/p2p", (SMSGatewayClient sms, [FromBody] List<P2PMessage> mobileContentList) =>
{
    return sms.P2P(new P2PRequest() { Extno = "1069012345", mobileContentList = mobileContentList });
});
app.MapGet("/Balance", (SMSGatewayClient sms) =>
{
    return sms.Balance(new BaseRequest() { });
});

app.MapGet("/Report", (SMSGatewayClient sms) =>
{
    return sms.Report(new ReportRequest() { });
});

app.MapGet("/Mo", (SMSGatewayClient sms) =>
{
    return sms.Mo(new ReportRequest() { });
});
app.MapPost("/Statis", (SMSGatewayClient sms, [FromBody] StatisRequest req) =>
{ 
    return sms.Statis(req);
});

```


### .NET FrameWork

安装包
```console
Install-Package IGeekFan.SMS57_SMGW
```

web.config中appSettings配置
```xml
<add key="SmsUrl" value="http://xxxx:8010" />
<add key="UserName" value="UserName" />
<add key="Password" value="Password" />
```

调用 

```csharp
[RoutePrefix("api/home")]
public class HomeController : ApiController
{
    private readonly SMGW_Option smsOption = new SMGW_Option()
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

    [Route("P2P")]
    [HttpPost]
    public P2PResponse P2P(List<P2PMessage> mobileContentList)
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.P2P(new P2PRequest() { Extno = "1069012345", mobileContentList = mobileContentList }); ;
    }

    [Route("Balance")]
    [HttpGet]
    public BalanceResponse Balance()
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.Balance(new BaseRequest() { });
    }

    [Route("Report")]
    [HttpGet]
    public ReportResponse Report()
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.Report(new ReportRequest() { });
    }

    [Route("Mo")]
    [HttpGet]
    public ReportResponse Mo()
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.Mo(new ReportRequest() { });
    }

    [Route("Statis")]
    [HttpPost]
    public StatisResponse Statis([FromBody] StatisRequest req)
    {
        var sms = new SMSGatewayClient(smsOption);
        return sms.Statis(req);
    }
}
```

## ASP.NET MVC 日志重写

默认会有`Trace.TraceInformation`处理日志，提示日志重写功能。继承接口`IGeekFan.SMS.Core.ILogger`，以下仅供参考，自动创建log目录并按日期记录日志，可自行实现 。

```csharp
/// <summary>
/// 自定义日志
/// </summary>
public class ErrLog : IGeekFan.SMS.Core.ILogger
{
    public static ErrLog Instance = new Lazy<ErrLog>(() => new ErrLog()).Value;

    protected ErrLog()
    {
    }

    public void LogInformation(string message)
    {
        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/log")))
        {
            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/log"));
        }
        string filename = HttpContext.Current.Server.MapPath("~/log/error" + DateTime.Now.ToString("yyyyMMdd") + ".log");
        TextWriter f = new StreamWriter(filename, true, Encoding.UTF8);
        f = TextWriter.Synchronized(f);
        f.WriteLine("LogInformation:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + message);
        f.Close();
    }
}
```
调用时，在实例化SMSGatewayClient时，指定`ErrLog.Instance`,在调用`Balance`方法时，内部记录的日志将调用自已的实现。
```csharp
[Route("Balance")]
[HttpGet]
public BalanceResponse Balance()
{
    var sms = new SMSGatewayClient(smsOption, ErrLog.Instance);
    return sms.Balance(new BaseRequest() { });
}
```

## 完整代码参考

- [/samples/SMS_ASPNET452/Controllers/HomeController.cs](../../samples/SMS_ASPNET452/Controllers/HomeController.cs)
- [/samples/SMS_ASPNET452/Controllers/ErrLog.cs](../../samples/SMS_ASPNET452/Controllers/ErrLog.cs)


## .NET Framework测试地址方法

配置完成后，运行SMS_ASPNET452项目。默认运行在端口`12762`上。

使用vscode安装扩展[https://marketplace.visualstudio.com/items?itemName=humao.rest-client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client),

用vscode打开此文件[SMS_ASPNET452.http](../../samples/SMS_ASPNET452.http)，并添加信任区，即可点击SendRequest调用接口

![../../docs/images/rest-client-use.png](../../docs/images/rest-client-use.png)