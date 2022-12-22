using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WatchDog.src.Helpers
{
    public static class CallerHelper
    {
        public static (string callerName, string filePath, int lineNumber) GetCaller(this StackFrame[] stackFrames)
        {
            for (int i = 0; i < stackFrames.Length; i++)
            {
                var method = stackFrames[i].GetMethod();
                var name = method?.DeclaringType?.Assembly.GetName().Name;

                switch (name)
                {
                    case "WatchDog":
                    case "Microsoft.Extensions.Logging":
                    case "Microsoft.Extensions.Logging.Abstractions":
                    case "System.Runtime.CompilerServices":
                        continue;
                }
                var filename = stackFrames[i].GetFileName();
                var line = stackFrames[i].GetFileLineNumber();

                return (method?.Name ?? string.Empty, stackFrames[i].GetFileName() ?? string.Empty, stackFrames[i].GetFileLineNumber());
            }
            return (string.Empty, string.Empty, 0);
        }
    }
}
