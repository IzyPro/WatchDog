using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WatchDog.src.Interfaces;

namespace WatchDog.src.Models
{
    public static class ServiceProviderFactory
    {
        public static IBroadcastHelper BroadcastHelper { get; set; }
    }
}
