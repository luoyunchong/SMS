using IGeekFan.MAS;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MASOption>(builder.Configuration.GetSection(nameof(MASOption)));
builder.Services.AddTransient<MASClient>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/sendmsg", (MASClient sms, [FromBody] SendMsgRequest sendMsg) =>
{
    return sms.SendMsg(sendMsg);
});

app.MapPost("/sendtmpsubmit", (MASClient sms, [FromBody] TmpsubmitRequest sendMsg) =>
{
    return sms.SendTmpsubmit(sendMsg);
});

app.MapGet("/getreport", (MASClient sms) =>
{
    return sms.GetReport();
});

app.MapGet("/getdeliver", (MASClient sms) =>
{
    return sms.GetDeliver();
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
/*
 {
  "rspcod": "success",
  "msgGroup": "0105013527000001139923",
  "success": true,
  "rspText": "数据验证通过。"
}
 */
app.Run();

