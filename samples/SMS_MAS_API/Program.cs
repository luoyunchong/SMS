using IGeekFan.MAS;
using Microsoft.AspNetCore.Mvc;

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

app.MapPost("/Send", (MASClient sms,[FromBody] SendMsgRequest sendMsg) =>
{
    return sms.SendMsg(sendMsg);
});

app.Run();

