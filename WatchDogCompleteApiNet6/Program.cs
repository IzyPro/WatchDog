using System.Text.Json;
using System.Text.Json.Serialization;
using WatchDog;
using WatchDog.src.Enums;
using WatchDog.src.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//builder.Services.AddWatchDogServices();
builder.Services.AddWatchDogServices(opt =>
{
    opt.IsAutoClear = true;
    opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
    opt.SqlDriverOption = WatchDogSqlDriverEnum.MSSQL;
    opt.SetExternalDbConnString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=test;";
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// optional mail notification
// var mailKitConfig = new MailKitConfig();
// mailKitConfig.mailKitClient = new MailKitClient()
// {
//    account = "input@gmail.com",
//    password = "xxxxxxxxxx",
//    useSsl = false,
//    port = 587,
//    hostUrl = "smtp.gmail.com"
// };
// mailKitConfig.mailSetting = new MailSetting()
// {
//    senderName = "testSender",
//    senderAddress = "xxxxxxxxx@gmail.com",
//    receiverName = "testReceiver",
//    receiverAddress = "xxxxxxxx@gmail.com",
//    subject = "testSMTP",
// };
// app.UseWatchDogExceptionLogger(mailKitConfig);

app.UseWatchDogExceptionLogger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWatchDog(conf =>
{
    conf.WatchPageUsername = "admin";
    conf.WatchPagePassword = "Qwerty@123";
    conf.Blacklist = "/auth";
});

app.Run();
