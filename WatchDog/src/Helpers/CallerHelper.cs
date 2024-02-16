using System.Diagnostics;

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
                    case "Microsoft.Extensions.Logging":
                    case "Microsoft.Extensions.Logging.Abstractions":
                    case "System.Runtime.CompilerServices":
                    case "WatchDog":
                        continue;
                }

                return (method?.Name ?? string.Empty, stackFrames[i].GetFileName() ?? string.Empty, stackFrames[i].GetFileLineNumber());
            }
            return (string.Empty, string.Empty, 0);
        }
    }
}
