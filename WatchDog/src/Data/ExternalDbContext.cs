using Dapper;
using MySql.Data.MySqlClient;
using Npgsql;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using WatchDog.src.Enums;
using WatchDog.src.Exceptions;
using WatchDog.src.Models;
using WatchDog.src.Utilities;

namespace WatchDog.src.Data
{
    internal static class ExternalDbContext
    {
        private static string _connectionString = WatchDogExternalDbConfig.ConnectionString;

        public static IDbConnection CreateConnection()
            => WatchDogSqlDriverOption.SqlDriverOption switch
            {
                WatchDogSqlDriverEnum.MSSQL => CreateMSSQLConnection(),
                WatchDogSqlDriverEnum.MySql => CreateMySQLConnection(),
                WatchDogSqlDriverEnum.PostgreSql => CreatePostgresConnection(),
                _ => throw new NotSupportedException()
            };


        public static void Migrate() => BootstrapTables();

        public static void BootstrapTables()
        {
            var createWatchTablesQuery = GetSqlQueryString();

            using (var connection = CreateConnection())
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
                    throw ae;
                }
                catch (Exception ex)
                {
                    throw new WatchDogDatabaseException(ex.Message);
                }
            }
        }

        public static string GetSqlQueryString() =>
            WatchDogSqlDriverOption.SqlDriverOption switch
            {
                WatchDogSqlDriverEnum.MSSQL => @$"
                                  IF OBJECT_ID('dbo.{Constants.WatchLogTableName}', 'U') IS NULL CREATE TABLE {Constants.WatchLogTableName} (
                                  id              INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                  responseBody    VARCHAR(max),
                                  responseStatus  int NOT NULL,
                                  requestBody     VARCHAR(30),
                                  queryString     VARCHAR(30),
                                  path            VARCHAR(30),
                                  requestHeaders  VARCHAR(max),
                                  responseHeaders VARCHAR(30),
                                  method          VARCHAR(30),
                                  host            VARCHAR(30),
                                  ipAddress       VARCHAR(30),
                                  timeSpent       VARCHAR(100),
                                  startTime       VARCHAR(100) NOT NULL,
                                  endTime         VARCHAR(100) NOT NULL
                            );
                                IF OBJECT_ID('dbo.{Constants.WatchLogExceptionTableName}', 'U') IS NULL CREATE TABLE {Constants.WatchLogExceptionTableName} (
                                id            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                                message       VARCHAR(max),
                                stackTrace    VARCHAR(max),
                                typeOf        VARCHAR(100),
                                source        VARCHAR(max),
                                path          VARCHAR(100),
                                method        VARCHAR(30),
                                queryString   VARCHAR(100),
                                requestBody   VARCHAR(max),
                                encounteredAt VARCHAR(100) NOT NULL
                             );
                        ",

                WatchDogSqlDriverEnum.MySql => @$"
                             CREATE TABLE IF NOT EXISTS {Constants.WatchLogTableName} (
                              id              INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                              responseBody    TEXT(65535),
                              responseStatus  INT NOT NULL,
                              requestBody     VARCHAR(30),
                              queryString     VARCHAR(30),
                              path            VARCHAR(30),
                              requestHeaders  TEXT(65535),
                              responseHeaders VARCHAR(30),
                              method          VARCHAR(30),
                              host            VARCHAR(30),
                              ipAddress       VARCHAR(30),
                              timeSpent       VARCHAR(100),
                              startTime       VARCHAR(100) NOT NULL,
                              endTime         VARCHAR(100) NOT NULL
                            );
                           CREATE TABLE IF NOT EXISTS {Constants.WatchLogExceptionTableName} (
                                id            INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
                                message       TEXT(65535),
                                stackTrace    TEXT(65535),
                                typeOf        VARCHAR(100),
                                source        TEXT(65535),
                                path          VARCHAR(100),
                                method        VARCHAR(30),
                                queryString   VARCHAR(100),
                                requestBody   TEXT(65535),
                                encounteredAt VARCHAR(100) NOT NULL
                             );
                        ",

                WatchDogSqlDriverEnum.PostgreSql => @$"
                             CREATE TABLE IF NOT EXISTS {Constants.WatchLogTableName} (
                              id              SERIAL PRIMARY KEY,
                              responseBody    VARCHAR,
                              responseStatus  int NOT NULL,
                              requestBody     VARCHAR(30),
                              queryString     VARCHAR(30),
                              path            VARCHAR(30),
                              requestHeaders  VARCHAR,
                              responseHeaders VARCHAR(30),
                              method          VARCHAR(30),
                              host            VARCHAR(30),
                              ipAddress       VARCHAR(30),
                              timeSpent       VARCHAR,
                              startTime       VARCHAR NOT NULL,
                              endTime         VARCHAR NOT NULL
                            );
                           CREATE TABLE IF NOT EXISTS {Constants.WatchLogExceptionTableName} (
                                id            SERIAL PRIMARY KEY,
                                message       VARCHAR,
                                stackTrace    VARCHAR,
                                typeOf        VARCHAR(100),
                                source        VARCHAR,
                                path          VARCHAR(100),
                                method        VARCHAR(30),
                                queryString   VARCHAR(100),
                                requestBody   VARCHAR,
                                encounteredAt VARCHAR NOT NULL
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
    }
}
