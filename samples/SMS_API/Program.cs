using IGeekFan.SMS57_SMGW;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SMGW_Option>(builder.Configuration.GetSection("SMGW_Option"));

builder.Services.AddTransient<SMSGatewayClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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


app.Run();
