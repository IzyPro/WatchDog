using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;
using WatchDog.src.Models;

namespace WatchDog.src.Helpers
{
    public static class LiteDBHelper
    {
        public static LiteDatabase db = new LiteDatabase("watchlogs.db");
        static ILiteCollection<WatchLog> _db = db.GetCollection<WatchLog>("Logs");

        public static IEnumerable<WatchLog> GetAll()
        {
            return _db.FindAll();
        }

        public static WatchLog GetById(int id)
        {
            return _db.FindById(id);
        }

        public static int Insert(WatchLog log)
        {
            return _db.Insert(log);
        }

        public static bool Update(WatchLog log)
        {
            return _db.Update(log);
        }

        public static bool Delete(int id)
        {
            return _db.Delete(id);
        }
    }
}
