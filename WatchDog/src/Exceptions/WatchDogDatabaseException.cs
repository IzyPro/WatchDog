using System;

namespace WatchDog.src.Exceptions
{
    internal class WatchDogDatabaseException : Exception
    {
        internal WatchDogDatabaseException() { }

        internal WatchDogDatabaseException(string message)
            : base(String.Format("WatchDog Database Exception: {0} Ensure you have passed the right Database Driver Option or a proper connection string at .AddWatchDogServices() as well as all required parameters for the database connection string", message))
        {

        }

        
    }
}
