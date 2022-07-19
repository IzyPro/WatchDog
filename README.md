# ![WatchDog Logo](https://github.com/IzyPro/WatchDog/blob/main/WatchDog/src/WatchPage/images/watchdogWhiteLogo.png)
# WatchDog


## Introduction

WatchDog is a Realtime Message, Event, HTTP (Request & Response) and Exception logger and viewer for ASP.Net Core Web Apps and APIs. It allows developers log and view messages, events, http requests made to their web application and also exception caught during runtime in their web applications, all in Realtime.
It leverages `SignalR` for real-time monitoring and `LiteDb` a Serverless MongoDB-like database with no configuration with the option of using your external MSSQL, MySQl or Postgres databases.

# ![Request & Response Viewer](https://github.com/IzyPro/WatchDog/blob/main/watchlog.png)

## General Features

- RealTime HTTP Request and Response Logger
- RealTime Exception Logger
- In-code message and eevent logging
- User Friendly Logger Views
- Search Option for HTTP and Exception Logs
- Filtering Option for HTTP Logs using HTTP Methods and StatusCode
- Logger View Authentication
- Auto Clear Logs Option

## What's New

- In-code logger for messages and events

## Fixes

- Fixed Middleware order
- Fixed Index pages not showing on MVC Apps


## Support
- .NET Core 3.1 and newer

## Installation

Install via .NET CLI

```bash
dotnet add package WatchDog.NET --version 1.3.0
```
Install via Package Manager

```bash
Install-Package WatchDog.NET --version 1.3.0
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
```c#
services.AddWatchDogServices(opt => 
{ 
   opt.IsAutoClear = true; 
});
```



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

### Setup Logging to External Db (MSSQL, MySQL & PostgreSQL) `Optional`
Add Database Connection String and Choose SqlDriver Option

```c#
services.AddWatchDogServices(opt => 
{
   opt.IsAutoClear = false; 
   opt.SetExternalDbConnString = "Server=localhost;Database=testDb;User Id=postgres;Password=root;"; 
   opt.SqlDriverOption = WatchDogSqlDriverEnum.PostgreSql; 
});
```



### Add WatchDog middleware in the HTTP request pipeline in `Startup.cs` under `Configure()`
# ![Login page sample](https://github.com/IzyPro/WatchDog/blob/main/login.png)

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
>If your projects startup or program class contains app.UseMvc() or app.UseRouting() then app.UseWatchDog() should come after `Important`
>If your projects startup or program class contains app.UseEndpoints() then app.UseWatchDog() should come before `Important`

# ![Request and Response Sample Details](https://github.com/IzyPro/WatchDog/blob/main/requestLog.png)

#### Add list of routes you want to ignore by the logger: `Optional`
List of routes, paths or specific strings to be ignored should be a comma separated string like below.

```c#
app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   opt.Blacklist = "Test/testPost, weatherforecast";
 });
```

#### Add WatchDog Exception Logger `Optional`
This is used to log in-app exceptions that occur during a particular HTTP request.
# ![Exception Sample Details](https://github.com/IzyPro/WatchDog/blob/main/exceptionLog.png)

>**NOTE**
>Add Exception Logger before the main WatchDog Middleware, preferably at the top of the middleware hierarchy so as to catch possible early exceptions.


```c#
app.UseWatchDogExceptionLogger();

...

app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   opt.Blacklist = "Test/testPost, weatherforecast";
 });
```
### Log Messages/Events
```
WatchLogger.Log("...TestGet Started...");
```
# ![In-code log messages](https://github.com/IzyPro/WatchDog/blob/main/in-code.jpeg)


### View Logs and Exception
Start your server and head to `/watchdog` to view the logs.
>Example: https://myserver.com/watchdog or https://localhost:[your-port]/watchdog

Still confused? Check out the implementation in the [WatchDogCompleteTestAPI](https://github.com/IzyPro/WatchDog/tree/main/WatchDogCompleteTestAPI) folder or the .NET 6 implementation in the [WatchDogCompleteApiNet6](https://github.com/IzyPro/WatchDog/tree/main/WatchDogCompleteApiNet6) folder.

## Contribution
Feel like something is missing? Fork the repo and send a PR.

Encountered a bug? Fork the repo and send a PR.

Alternatively, open an issue and we'll get to it as soon as we can.

## Credit
Kelechi Onyekwere -  [Github](https://github.com/Khelechy) [Twitter](https://twitter.com/khelechy1337)

Israel Ulelu - [Github](https://github.com/IzyPro) [Twitter](https://twitter.com/IzyPro_)
