
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
| `IGeekFan.SMSUnicom` | [![NuGet](https://img.shields.io/nuget/v/IGeekFan.SMSUnicom.svg?style=flat-square&label=nuget&color=fedcba)](https://www.nuget.org/packages/IGeekFan.SMSUnicom/) | ![downloads](https://img.shields.io/nuget/dt/IGeekFan.SMSUnicom.svg) |.NET4.5.2/.NETStandard2.0

## IGeekFan.SMS57_SMGW

实现SMS 企信通短信、即同求宝[http://tqiu.cn/](http://tqiu.cn/)的短信发送SDK ，[使用文档点击此链接](./src/SMS57_SMGW/README.md)
- ASP.NET Core6项目示例代码：[samples/SMS_API](./samples/SMS_API/Program.cs)
- ASP.NET MVC 452 项目示例代码：[samples/SMS_ASPNET452](./samples/SMS_ASPNET452/Controllers/HomeController.cs)
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

## IGeekFan.SMSUnicom

[融合云信]([https://maap.wo.cn/index.html](https://maap.wo.cn/index.html))短信发送SDK ，[使用文档点击此链接](./src/IGeekFan.SMSUnicom/README.md)
- ASP.NET Core6项目示例代码：[samples/SMSUnicom_API](./samples/SMSUnicom_API/Program.cs)

融合云信是**中国联通**的短信平台，提供百亿发送量，稳定的平台服务。
- 海量发送能力：三大运营商短信全网覆盖，全国用户畅行无阻，支持短信双向收发
- 组织管理功能：提供多账号的统一计费、权限控制等能力，支持多层级管理模式
- 智能运维&立体化监控实时监控：精准掌握服务健康状况、服务拓扑，调用链跟踪可视化呈现、多维度关联分析，预防系统级故障