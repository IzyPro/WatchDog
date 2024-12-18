using Dapper;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data;
using System.Diagnostics;
using WatchDog.src.Enums;
using WatchDog.src.Exceptions;
using WatchDog.src.Models;
using WatchDog.src.Utilities;
using Microsoft.Data.SqlClient;

namespace WatchDog.src.Data
{
    internal static class ExternalDbContext
    {
        private static string _connectionString = WatchDogExternalDbConfig.ConnectionString;

        public static IDbConnection CreateSQLConnection()
            => WatchDogDatabaseDriverOption.DatabaseDriverOption switch
            {
                WatchDogDbDriverEnum.MSSQL => CreateMSSQLConnection(),
                WatchDogDbDriverEnum.MySql => CreateMySQLConnection(),
                WatchDogDbDriverEnum.PostgreSql => CreatePostgresConnection(),
                _ => throw new NotSupportedException()
            };

        public static void Migrate() => BootstrapTables();

        public static void BootstrapTables()
        {
            var createWatchTablesQuery = GetSqlQueryString();

            using (var connection = CreateSQLConnection())
            {
                try
                {
                    connection.Open();
                    _ =  connection.Query(createWatchTablesQuery);
                    connection.Close();
                }
                catch (SqlException ae)
                {
                    Debug.WriteLine(ae.Message.ToString());
#pragma warning disable CA2200 // Rethrow to preserve stack details
                    throw ae;
#pragma warning restore CA2200 // Rethrow to preserve stack details
                }
                catch (Exception ex)
                {
                    throw new WatchDogDatabaseException(ex.Message);
                }
            }

        }

        public static void MigrateNoSql()
        {
            try
            {
                var mongoClient = CreateMongoDBConnection();
                var database = mongoClient.GetDatabase(WatchDogExternalDbConfig.MongoDbName);
                _ = database.GetCollection<WatchLog>(Constants.WatchLogTableName);
                _ = database.GetCollection<WatchExceptionLog>(Constants.WatchLogExceptionTableName);
                _ = database.GetCollection<WatchLoggerModel>(Constants.LogsTableName);

                //Seed counterDb
                var filter = new BsonDocument("name", Constants.WatchDogMongoCounterTableName);

                // Check if the collection exists
                var collections = database.ListCollections(new ListCollectionsOptions { Filter = filter });

                bool exists = collections.Any();
                var _counter = database.GetCollection<Sequence>(Constants.WatchDogMongoCounterTableName);

                if (!exists)
                {
                    var sequence = new Sequence
                    {
                        _Id = "sequenceId",
                        Value = 0
                    };
                    _counter.InsertOne(sequence);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message.ToString());
                throw new WatchDogDatabaseException(ex.Message);
            }
        }

        public static string GetSqlQueryString() =>
            WatchDogDatabaseDriverOption.DatabaseDriverOption switch
            {
                WatchDogDbDriverEnum.MSSQL => @$"
                                  IF OBJECT_ID('dbo.{Constants.WatchLogTableName}', 'U') IS NULL CREATE TABLE {Constants.WatchLogTableName} (
                                  id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                  responseBody    VARCHAR(max),
                                  responseStatus  int NOT NULL,
                                  requestBody     VARCHAR(max),
                                  queryString     VARCHAR(max),
                                  path            VARCHAR(max),
                                  requestHeaders  VARCHAR(max),
                                  responseHeaders VARCHAR(max),
                                  method          VARCHAR(30),
                                  host            VARCHAR(max),
                                  ipAddress       VARCHAR(30),
                                  timeSpent       VARCHAR(100),
                                  startTime       VARCHAR(100) NOT NULL,
                                  endTime         VARCHAR(100) NOT NULL
                            );
                                IF OBJECT_ID('dbo.{Constants.WatchLogExceptionTableName}', 'U') IS NULL CREATE TABLE {Constants.WatchLogExceptionTableName} (
                                id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                message       VARCHAR(max),
                                stackTrace    VARCHAR(max),
                                typeOf        VARCHAR(max),
                                source        VARCHAR(max),
                                path          VARCHAR(max),
                                method        VARCHAR(30),
                                queryString   VARCHAR(max),
                                requestBody   VARCHAR(max),
                                encounteredAt VARCHAR(100) NOT NULL
                             );
                                IF OBJECT_ID('dbo.{Constants.LogsTableName}', 'U') IS NULL CREATE TABLE {Constants.LogsTableName} (
                                id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                eventId       VARCHAR(100),
                                message       VARCHAR(max),
                                timestamp     VARCHAR(100) NOT NULL,
                                callingFrom   VARCHAR(100),
                                callingMethod VARCHAR(100),
                                lineNumber    INT,
                                logLevel      VARCHAR(30)
                             );
                        ",

                WatchDogDbDriverEnum.MySql => @$"
                             CREATE TABLE IF NOT EXISTS {Constants.WatchLogTableName} (
                              id              INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                              responseBody    TEXT(65535),
                              responseStatus  INT NOT NULL,
                              requestBody     TEXT(65535),
                              queryString     VARCHAR(65535),
                              path            VARCHAR(65535),
                              requestHeaders  TEXT(65535),
                              responseHeaders VARCHAR(65535),
                              method          VARCHAR(30),
                              host            VARCHAR(65535),
                              ipAddress       VARCHAR(30),
                              timeSpent       VARCHAR(100),
                              startTime       VARCHAR(100) NOT NULL,
                              endTime         VARCHAR(100) NOT NULL
                            );
                           CREATE TABLE IF NOT EXISTS {Constants.WatchLogExceptionTableName} (
                                id            INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                                message       TEXT(65535),
                                stackTrace    TEXT(65535),
                                typeOf        VARCHAR(65535),
                                source        TEXT(65535),
                                path          VARCHAR(65535),
                                method        VARCHAR(30),
                                queryString   VARCHAR(65535),
                                requestBody   TEXT(65535),
                                encounteredAt VARCHAR(100) NOT NULL
                             );
                           CREATE TABLE IF NOT EXISTS {Constants.LogsTableName} (
                                id            INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                                eventId       VARCHAR(100),
                                message       TEXT(65535),
                                timestamp     VARCHAR(100) NOT NULL,
                                callingFrom   VARCHAR(100),
                                callingMethod VARCHAR(100),
                                lineNumber    INT,
                                logLevel      VARCHAR(30)
                             );
                        ",

                WatchDogDbDriverEnum.PostgreSql => @$"
                             CREATE TABLE IF NOT EXISTS {Constants.WatchLogTableName} (
                              id              SERIAL PRIMARY KEY,
                              responseBody    VARCHAR,
                              responseStatus  int NOT NULL,
                              requestBody     VARCHAR,
                              queryString     VARCHAR,
                              path            VARCHAR,
                              requestHeaders  VARCHAR,
                              responseHeaders VARCHAR,
                              method          VARCHAR(30),
                              host            VARCHAR,
                              ipAddress       VARCHAR(30),
                              timeSpent       VARCHAR,
                              startTime       TIMESTAMP with time zone NOT NULL,
                              endTime         TIMESTAMP with time zone NOT NULL
                            );
                           CREATE TABLE IF NOT EXISTS {Constants.WatchLogExceptionTableName} (
                                id            SERIAL PRIMARY KEY,
                                message       VARCHAR,
                                stackTrace    VARCHAR,
                                typeOf        VARCHAR,
                                source        VARCHAR,
                                path          VARCHAR,
                                method        VARCHAR(30),
                                queryString   VARCHAR,
                                requestBody   VARCHAR,
                                encounteredAt TIMESTAMP with time zone NOT NULL
                             );
                           CREATE TABLE IF NOT EXISTS {Constants.LogsTableName} (
                                id            SERIAL PRIMARY KEY,
                                eventId       VARCHAR(100),
                                message       VARCHAR,
                                timestamp     TIMESTAMP with time zone NOT NULL,
                                callingFrom   VARCHAR,
                                callingMethod VARCHAR(100),
                                lineNumber    INTEGER,
                                logLevel      VARCHAR(30)
                             );
                        ",
                _ => ""
            };

        public static NpgsqlConnection CreatePostgresConnection()
        {
            try
            {
                return new NpgsqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new WatchDogDatabaseException(ex.Message);
            }
        }

        public static MySqlConnection CreateMySQLConnection()
        {
            try
            {
                return new MySqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new WatchDogDatabaseException(ex.Message);
            }
        }

        public static SqlConnection CreateMSSQLConnection()
        {
            try
            {
                return new SqlConnection(_connectionString);
            }
            catch (Exception ex)
            {
                throw new WatchDogDatabaseException(ex.Message);
            }
        }

        public static MongoClient CreateMongoDBConnection()
        {
            try
            {
                return new MongoClient(_connectionString);
            }
            catch (Exception ex)
            {
                throw new WatchDogDatabaseException(ex.Message);
            }
        }
    }
}
