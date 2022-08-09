using System;

namespace WatchDog.src.Exceptions
{
    internal class WatchDogDBDriverException : Exception
    {
        internal WatchDogDBDriverException(string message)
            : base(String.Format("WatchDog Database Exception: {0}", message))
        {

        }
    }
}
