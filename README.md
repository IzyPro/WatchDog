# ![WatchDog Logo](https://github.com/IzyPro/WatchDog/blob/main/WatchDog/src/WatchPage/images/watchdogWhiteLogo.png)

# WatchDog

[![WatchDog](https://img.shields.io/badge/WatchDog-blueviolet)](https://github.com/IzyPro/WatchDog)
[![Version](https://img.shields.io/nuget/vpre/WatchDog.NET?color=orange)](https://www.nuget.org/packages/WatchDog.NET#versions-tab)
[![Downloads](https://img.shields.io/nuget/dt/WatchDog.NET?color=red)](https://www.nuget.org/packages/WatchDog.NET#versions-tab)
[![MIT License](https://img.shields.io/github/license/IzyPro/WatchDog?color=Green)](https://github.com/IzyPro/WatchDog/blob/main/LICENSE)
[![WatchDog](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Fgithub.com%2FIzyPro%2FWatchDog)](https://twitter.com/intent/tweet?hashtags=WatchDog&original_referer=https%3A%2F%2Fdeveloper.twitter.com%2F&ref_src=twsrc%5Etfw%7Ctwcamp%5Ebuttonembed%7Ctwterm%5Eshare%7Ctwgr%5E&related=twitterapi%2Ctwitter&text=Hello%2C%20world!%0DCheck%20out%20this%20awesome%20developer%20tool&url=https%3A%2F%2Fgithub.com%2FIzyPro%2FWatchDog&via=HQWatchdog)

## Introduction

WatchDog is a Realtime Message, Event, HTTP (Request & Response) and Exception logger and viewer for ASP.Net Core Web Apps and APIs. It allows developers log and view messages, events, http requests made to their web application and also exception caught during runtime in their web applications, all in Realtime.
It leverages `SignalR` for real-time monitoring and `LiteDb` a Serverless MongoDB-like database with no configuration with the option of using your external databases (MSSQL, MySQl, Postgres, MongoDB).

![Request & Response Viewer](https://github.com/IzyPro/WatchDog/blob/main/static/images/watchlog.png)

## General Features

- RealTime HTTP Request, Response, and Exception Logger
- In-code message and event logging
- User Friendly Logger Views
- Search Option for HTTP and Exception Logs
- Filtering Option for HTTP Logs using HTTP Methods and StatusCode
- Logger View Authentication
- Auto Clear Logs Option

## What's New

- Security Optimization
- Query Filters Fixes and Optimizations
- Package Assembly as DB Name Fix(MongoDB)

### Breaking Changes

- SqlDriverOption is now DbDriverOption (>= 1.4.0)

## Support

- .NET Core 3.1 and newer

## Installation

Install via .NET CLI

```bash
dotnet add package WatchDog.NET --version 1.4.10
```

Install via Package Manager

```bash
Install-Package WatchDog.NET --version 1.4.10
```

## Usage

To enable WatchDog to listen for requests, use the WatchDog middleware provided by WatchDog.

Add WatchDog Namespace in `Startup.cs`

```c#
using WatchDog;
```

### Register WatchDog service in `Startup.cs` under `ConfigureService()`

```c#
services.AddWatchDogServices();
```

### Setup AutoClear Logs `Optional`

This clears the logs after a specific duration.
>**NOTE**
>When `IsAutoClear = true`
>Default Schedule Time is set to Weekly,  override the settings like below:

```c#
services.AddWatchDogServices(opt => 
{ 
   opt.IsAutoClear = true;
   opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
});
```

### Setup Logging to External Db (MSSQL, MySQL, PostgreSQL & MongoDb) `Optional`

Add Database Connection String and Choose DbDriver Option

```c#
services.AddWatchDogServices(opt => 
{
   opt.IsAutoClear = false; 
   opt.SetExternalDbConnString = "Server=localhost;Database=testDb;User Id=postgres;Password=root;"; 
   opt.DbDriverOption = WatchDogSqlDriverEnum.PostgreSql; 
});
```

### Add WatchDog middleware in the HTTP request pipeline in `Startup.cs` under `Configure()`

# ![Login page sample](https://github.com/IzyPro/WatchDog/blob/main/static/images/login.png)

>**NOTE**
>Add Authentication option like below: `Important`

This authentication information (Username and Password) will be used to access the log viewer.

```c#
app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
 });
```

>**NOTE**
> If your project uses authentication, then `app.UseWatchDog();` should come after app.UseRouting(), app.UseAuthentication(), app.UseAuthorization(), in that order
<!--- >If your projects startup or program class contains app.UseMvc() or app.UseRouting() then app.UseWatchDog() should come after `Important`
>If your projects startup or program class contains app.UseEndpoints() then app.UseWatchDog() should come before `Important` -->

# ![Request and Response Sample Details](https://github.com/IzyPro/WatchDog/blob/main/static/images/requestLog.png)

#### Optional Configurations: `Optional`

- Blacklist: List of routes, paths or endpoints to be ignored (should be a comma separated string like below).
- Serializer: If not default, specify the type of global json serializer/converter used
- CorsPolicy: Policy Name if project uses CORS

```c#
app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   //Optional
   opt.Blacklist = "Test/testPost, api/auth/login"; //Prevent logging for specified endpoints
   opt.Serializer = WatchDogSerializerEnum.Newtonsoft; //If your project use a global json converter
   opt.CorsPolicy = "MyCorsPolicy"
 });
```

#### Add WatchDog Exception Logger `Optional`

This is used to log in-app exceptions that occur during a particular HTTP request.

# ![Exception Sample Details](https://github.com/IzyPro/WatchDog/blob/main/static/images/exceptionLog.png)

>**NOTE**
>Add Exception Logger before the main WatchDog Middleware, preferably at the top of the middleware hierarchy so as to catch possible early exceptions.

```csharp
app.UseWatchDogExceptionLogger();

...

app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   ...
 });
```

### Log Messages/Events

```csharp
WatchLogger.Log("...Test Log...");
WatchLogger.LogWarning(JsonConvert.Serialize(model));
WatchLogger.LogError(res.Content, eventId: reference);
```

# ![In-code log messages](https://github.com/IzyPro/WatchDog/blob/main/in-code.png)

#### Sink Logs from ILogger

You can also sink logs from the .NET ILogger into WatchDog

For .NET 6 and above

```csharp
builder.Logging.AddWatchDogLogger();
```

For .NET Core 3.1, configure logging and add `.AddWatchDogLogger()` to the `CreateHostBuilder` method of the `Program.cs` class

```csharp
Host.CreateDefaultBuilder(args)
 .ConfigureLogging( logging =>
 {
     logging.AddWatchDogLogger();
 })
 .ConfigureWebHostDefaults(webBuilder =>
 {
     webBuilder.UseStartup<Startup>();
 });
```

### View Logs and Exception

Start your server and head to `/watchdog` to view the logs.
>Example: <https://myserver.com/watchdog> or <https://localhost:[your-port]/watchdog>

Still confused? Check out the implementation in the [WatchDogCompleteTestAPI](https://github.com/IzyPro/WatchDog/tree/main/WatchDogCompleteTestAPI) folder or the .NET 6 implementation in the [WatchDogCompleteApiNet6](https://github.com/IzyPro/WatchDog/tree/main/WatchDogCompleteApiNet6) folder.

## Contribution

Feel like something is missing? Fork the repo and send a PR.

Encountered a bug? Fork the repo and send a PR.

Alternatively, open an issue and we'll get to it as soon as we can.

## Credit

Kelechi Onyekwere -  [Github](https://github.com/Khelechy) [Twitter](https://twitter.com/khelechy1337)

Israel Ulelu - [Github](https://github.com/IzyPro) [Twitter](https://twitter.com/IzyPro_)

### [Official Documentation](https://watchdog-3.gitbook.io/watchdog)
