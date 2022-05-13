using System;

namespace WatchDog.src.Exceptions
{
    internal class WatchDogAuthenticationException : Exception
    {
        internal WatchDogAuthenticationException() { }

        internal WatchDogAuthenticationException(string message)
            : base(String.Format("WatchDog Authentication Exception: {0}", message))
        {

        }
    }
}
