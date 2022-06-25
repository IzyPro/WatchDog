using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WatchDog.src.Models
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; set; }
    }
}
