using System;

namespace WatchDog.src.Exceptions
{
    internal class WatchDogDatabaseException : Exception
    {
        internal WatchDogDatabaseException() { }

        internal WatchDogDatabaseException(string message)
            : base(String.Format("WatchDog Database Exception: {0} Ensure you have passed the right SQLDriverOption at .AddWatchDogServices() as well as all required parameters for the database connection string", message))
        {

        }
    }
}
