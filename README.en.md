**English** | [简体中文](README.md)


<div align="center">
<h1 align="center"> <img alt="logo" src="./docs/images/logo.png" width="40px" />  SMS </h1>

**SMS** provides additional SMS provider implementations for .NET projects.

[![.NET IDE Rider](https://img.shields.io/static/v1?style=float&logo=rider&label=Rider&message=jetbrains&color=red)](https://www.jetbrains.com/rider/)
[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/luoyunchong/SMS/master/LICENSE)
</div>

## Nuget Packages

| Package name| Version| Downloads    |Support|
|  --------- |  --------- |  --------- | ---------
| `IGeekFan.SMS57_SMGW` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.SMS57_SMGW.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.SMS57_SMGW/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.SMS57_SMGW.svg) |.NET4.5.2/.NETStandard2.0
| `IGeekFan.SMSUnicom` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.SMSUnicom.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.SMSUnicom/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.SMSUnicom.svg) |.NET4.5.2/.NETStandard2.0
| `IGeekFan.MAS` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.MAS.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.MAS/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.MAS.svg) |.NET4.5.2/.NETStandard2.0

## IGeekFan.SMS57_SMGW

An SDK for sending SMS through Qixintong (Tongqiubao): [http://tqiu.cn/](http://tqiu.cn/). See the [usage documentation](./src/SMS57_SMGW/README.md).
- ASP.NET Core6 sample code：[samples/SMS_API](./samples/SMS_API/Program.cs)
- ASP.NET MVC 452  sample code：[samples/SMS_ASPNET452](./samples/SMS_ASPNET452/Controllers/HomeController.cs)
- SMGW stands for SMS gateway
- SMGW: SMS Gateway

```
Send URL：http(s)://ip:port/api/sm57/sms?action=send
Peer-to-peer URL：http://host:port/sms?action=p2p
Balance URL：http(s)://ip:port/api/sm57/sms?action=balance
Report URL：http(s)://ip:port/api/sm57/sms?action=report
Inbound URL：http(s)://ip:port/api/sm57/sms?action=mo
Statistics URL：http://host:port/api/sm57/sms?action=statis
```

## IGeekFan.SMSUnicom

SMS SDK for [Ronghe Yunxin](https://maap.wo.cn/index.html). See the [usage documentation](./src/IGeekFan.SMSUnicom/README.md).
- ASP.NET Core6 sample code：[samples/SMSUnicom_API](./samples/SMSUnicom_API/Program.cs)

Ronghe Yunxin is **China Unicom**'s SMS platform, offering stable service at large scale.
- Large-scale delivery: coverage across China's three major carriers, with two-way SMS support
- Organization management: unified billing and access controls for multiple accounts, with multilevel management
- Operations and monitoring: service health, topology, visualized call tracing, and multidimensional analysis to help prevent system failures

## IGeekFan.MAS

SMS SDK for the [Cloud MAS platform](https://mas.10086.cn/login). See the [usage documentation](./src/IGeekFan.MAS/README.md).

China Mobile's Cloud MAS platform is used to send SMS and MMS messages.
