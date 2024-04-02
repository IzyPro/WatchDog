using System.Text.Json;
using System.Text.Json.Serialization;
using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddWatchDogServices();
builder.Services.AddWatchDogServices(opt =>
{
    //opt.IsAutoClear = true; // This is a trick, it doesn't leave a history of a few days
    //opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Hourly;    
    //opt.SetExternalDbConnString = "Server=lucky.db.elephantsql.com;Database=kfmlwanq;User Id=kfmlwanq;Password=jYpwgweV43BUr51SDcPHWdCfEmhpvQPz;";
    //opt.SetExternalDbConnString = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=test;";
    
    opt.DbDriverOption = WatchDogDbDriverEnum.Mongo;
    opt.SetExternalDbConnString = "mongodb://172.16.3.148"; // "mongodb://localhost:27017";
    opt.MongoDbName = "watch_dev";
    
    //opt.DbDriverOption = WatchDogDbDriverEnum.MSSQL;
    //opt.SetExternalDbConnString = "server=localhost,1433;pwd=UsjUe5STTgQcmvB9;uid=sa;database=Watch;Application Name=SalesBackendDev;TrustServerCertificate=True;";

    // opt.DbDriverOption = WatchDogDbDriverEnum.MSSQL;
    // opt.SetExternalDbConnString = "server=172.16.3.74;pwd=7p@Py2V73$c0HBnz;uid=wdog;database=Watch;Application Name=CswSettingsWillys;TrustServerCertificate=True;";
    
    // opt.DbDriverOption = WatchDogDbDriverEnum.PostgreSql;
    // opt.SetExternalDbConnString = "Host=localhost;Username=sa;Password=UsjUe5STTgQcmvB9;Database=Watch;Port=5432";
});
builder.Logging.AddWatchDogLogger();

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

app.UseWatchDogExceptionLogger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWatchDog(conf =>
{
    conf.WatchPageUsername = "admin";
    conf.WatchPagePassword = "admin";
    conf.Blacklist = "/auth";
    conf.Tag = "SalesBackend";
    conf.HeaderNameEventId = "EventId";
});

app.Run();
