using IGeekFan.SMSUnicom;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SMSUnicomOption>(builder.Configuration.GetSection(typeof(SMSUnicomOption).Name));

builder.Services.AddTransient<SMSUnicomClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

/// <summary>
/// 同步方法：短信发送接口
/// </summary>
app.MapGet("/sendtempletmsg", (SMSUnicomClient smsClient, string mobiles, string msg) =>
{
    return smsClient.SendTempletMsg(
          new SendTempletMsgRequest()
          {
              Templetid = builder.Configuration.GetSection("SMSUnicomOption:SendMessageTemplateId").Value,
              Mobiles = mobiles,
              Msg = msg
          });
}).WithDisplayName("短信发送接口");

/// <summary>
/// 异步方法：短信发送
/// </summary>
app.MapGet("/sendtempletmsg-async", async (SMSUnicomClient smsClient, string mobiles, string msg) =>
{
    return await smsClient.SendTempletMsgAsync(
          new SendTempletMsgRequest()
          {
              Templetid = builder.Configuration.GetSection("SMSUnicomOption:SendMessageTemplateId").Value,
              Mobiles = mobiles,
              Msg = msg
          });
});

app.Run();

