
<div align="center">
<h1 align="center"> <img alt="logo" src="./docs/images/logo.png" width="40px" />  SMS </h1>

**SMS** 为.NET 项目提供了更多厂商短信的实现

[![.NET IDE Rider](https://img.shields.io/static/v1?style=float&logo=rider&label=Rider&message=jetbrains&color=red)](https://www.jetbrains.com/rider/)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/luoyunchong/SMS/master/LICENSE)
</div>

## Nuget Packages

| Package name| Version| Downloads    |Support|
|  --------- |  --------- |  --------- | ---------
| `IGeekFan.SMS57_SMGW` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.SMS57_SMGW.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.SMS57_SMGW/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.SMS57_SMGW.svg) |.NET4.5.2/.NETStandard2.0


## IGeekFan.SMS57_SMGW

实现SMS 企信通短信、即同求宝[http://tqiu.cn/](http://tqiu.cn/)的短信发送SDK ，[使用文档点击此链接](./src/SMS57_SMGW/README.md)
- ASP.NET Core6项目示例代码：[samples/SMS_API](./samples/SMS_API/Program.cs)
- ASP.NET MVC 452 项目示例代码：[samples/SMS_ASPNET452](./samples/SMS_ASPNET452/Controllers/HomeController.cs)
- (SMGW) 代指 短消息网关
- (SMGW) 即：SMS Mail Gateway

```
下发地址：http(s)://ip:port/api/sm57/sms?action=send
点对点地址：http://host:port/sms?action=p2p
余额地址：http(s)://ip:port/api/sm57/sms?action=balance
报告地址：http(s)://ip:port/api/sm57/sms?action=report
上行地址：http(s)://ip:port/api/sm57/sms?action=mo
统计地址：http://host:port/api/sm57/sms?action=statis
```