using WatchDog;
using WatchDog.src.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOutputCache();
//builder.Services.AddWatchDogServices();
builder.Services.AddWatchDogServices(opt => { opt.IsAutoClear = true; opt.SetExternalDbConnString = "Server=(localdb)\\mssqllocaldb;Database=test;Trusted_Connection=True;"; opt.DbDriverOption = WatchDogDbDriverEnum.MSSQL; });

var app = builder.Build();

app.UseWatchDogExceptionLogger();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseOutputCache();
app.UseWatchDog(conf =>
{
    conf.WatchPageUsername = "admin";
    conf.WatchPagePassword = "Qwerty@123";
    conf.Blacklist = "/auth, user";
    conf.UseOutputCache = true;
});

app.MapControllers();

app.Run();
