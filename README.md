# WatchDog


## Introduction

WatchDog is a HTTP (Request & Response) and Exception logger and viewer for ASP.Net Core Web Apps and APIs. It allows developers log and view http requests made to their and also exception caught during runtime in their web applications.
It leverages on LiteDb a Serverless MongoDB-like database with no configuration.
 
## Installation

Install via .NET CLI

```bash
dotnet add package WatchDog --version 1.0.0
```
Install via Package Manager

```bash
Install-Package WatchDog --version 1.0.0
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

```c#
services.AddWatchDogServices(opt => 
{ 
   opt.IsAutoClear = false; 
});
```

>**NOTE**</br>
>When `IsAutoClear = true`
>Default Schedule Time is set to Weekly,  override the settings like below:


```c#
services.AddWatchDogServices(opt => 
{ 
   opt.IsAutoClear = true;
   opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
});

### Add WatchDog middleware in the HTTP request pipeline in `Startup.cs` under `Configure()`

>**NOTE**</br>
>Add Authentication option like below: `Important`


```c#
app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
 });
```

#### Add list of routes you want to ignore by the logger, like below: `Optional`

```c#
app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   opt.Blacklist = "Test/testPost, weatherforecast";
 });
```

#### Add WatchDog Exception Logger `Optional`

>**NOTE**</br>
>Add Exception Logger before the main WatchDog Middleware

```c#
app.UseWatchDogExceptionLogger();

app.UseWatchDog(opt => 
{ 
   opt.WatchPageUsername = "admin"; 
   opt.WatchPagePassword = "Qwerty@123"; 
   opt.Blacklist = "Test/testPost, weatherforecast";
 });
```


## Contribution
