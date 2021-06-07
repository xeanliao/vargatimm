using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;

namespace GTU.DataLayer
{
    public class DatabaseHelper
    {
        //public const string ConnectionStringName = "GTUTrackingConnectionString";
        //public static GTUTrackDataContext GetDatabaseData()
        //{
        //    var db = new gtutrackDataContext(ConfigurationManager.ConnectionStrings[ConnectionStringName].ConnectionString);

        //    return db;
        //}

        //public static bool Insert<T>(T obj) where T : class
        //{
        //    using (var db = GetDatabaseData())
        //    {
        //        db.GetTable<T>().InsertOnSubmit(obj);
        //        db.SubmitChanges();
        //        return true;
        //    }
        //}

        //public static void Update<T>(T obj, Action<T> update) where T : class
        //{
        //    using (var db = GetDatabaseData())
        //    {
        //        db.GetTable<T>().Attach(obj);
        //        update(obj);
        //        db.SubmitChanges();
        //    }
        //}

        //public static void Delete<T>(T obj) where T : class, new()
        //{
        //    using (var db = GetDatabaseData())
        //    {
        //        db.GetTable<T>().Attach(obj);
        //        db.GetTable<T>().DeleteOnSubmit(obj);
        //        db.SubmitChanges();
        //    }
        //}
    }
}
