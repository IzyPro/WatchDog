using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WatchDog.src.Data;
using WatchDog.src.Models;
using WatchDog.src.Utilities;

namespace WatchDog.src.Helpers
{
    internal static class ExternalDbHelper
    {
        // WATCHLOG OPERATIONS
        public static async Task<IEnumerable<WatchLog>> GetAllWatchLogs()
        {
            var query = @$"SELECT * FROM {Constants.WatchLogTableName}";
            using (var connection = ExternalDbContext.CreateConnection())
            {
                connection.Open();
                var logs = await connection.QueryAsync<WatchLog>(query);
                connection.Close();
                return logs.AsList();
            }
        }

        public static async Task InsertWatchLog(WatchLog log)
        {
            var query = @$"INSERT INTO {Constants.WatchLogTableName} (responseBody,responseStatus,requestBody,queryString,path,requestHeaders,responseHeaders,method,host,ipAddress,timeSpent,startTime,endTime) " +
                "VALUES (@ResponseBody,@ResponseStatus,@RequestBody,@QueryString,@Path,@RequestHeaders,@ResponseHeaders,@Method,@Host,@IpAddress,@TimeSpent,@StartTime,@EndTime);";

            var parameters = new DynamicParameters();
            parameters.Add("ResponseBody", log.ResponseBody, DbType.String);
            parameters.Add("ResponseStatus", log.ResponseStatus, DbType.Int32);
            parameters.Add("RequestBody", log.RequestBody, DbType.String);
            parameters.Add("QueryString", log.QueryString, DbType.String);
            parameters.Add("Path", log.Path, DbType.String);
            parameters.Add("RequestHeaders", log.RequestHeaders, DbType.String);
            parameters.Add("ResponseHeaders", log.ResponseHeaders, DbType.String);
            parameters.Add("Method", log.Method, DbType.String);
            parameters.Add("Host", log.Host, DbType.String);
            parameters.Add("IpAddress", log.IpAddress, DbType.String);
            parameters.Add("TimeSpent", log.TimeSpent, DbType.String);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("StartTime", log.StartTime.ToUniversalTime(), DbType.DateTime);
                parameters.Add("EndTime", log.EndTime.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("StartTime", log.StartTime);
                parameters.Add("EndTime", log.EndTime);
            }

            using (var connection = ExternalDbContext.CreateConnection())
            {
                connection.Open();
                await connection.ExecuteAsync(query, parameters);
                connection.Close();
            }
        }



        // WATCH EXCEPTION OPERATIONS
        public static async Task<IEnumerable<WatchExceptionLog>> GetAllWatchExceptionLogs()
        {
            var query = @$"SELECT * FROM {Constants.WatchLogExceptionTableName}";
            using (var connection = ExternalDbContext.CreateConnection())
            {
                var logs = await connection.QueryAsync<WatchExceptionLog>(query);
                return logs.AsList();
            }
        }

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            var query = @$"INSERT INTO {Constants.WatchLogExceptionTableName} (message,stackTrace,typeOf,source,path,method,queryString,requestBody,encounteredAt) " +
                "VALUES (@Message,@StackTrace,@TypeOf,@Source,@Path,@Method,@QueryString,@RequestBody,@EncounteredAt);";

            var parameters = new DynamicParameters();
            parameters.Add("Message", log.Message, DbType.String);
            parameters.Add("StackTrace", log.StackTrace, DbType.String);
            parameters.Add("TypeOf", log.TypeOf, DbType.String);
            parameters.Add("Source", log.Source, DbType.String);
            parameters.Add("Path", log.Path, DbType.String);
            parameters.Add("Method", log.Method, DbType.String);
            parameters.Add("QueryString", log.QueryString, DbType.String);
            parameters.Add("RequestBody", log.RequestBody, DbType.String);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("EncounteredAt", log.EncounteredAt.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("EncounteredAt", log.EncounteredAt, DbType.DateTime);
            }

            using (var connection = ExternalDbContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        // LOGS OPERATION
        public static async Task<IEnumerable<WatchLoggerModel>> GetAllLogs()
        {
            var query = @$"SELECT * FROM {Constants.LogsTableName}";
            using (var connection = ExternalDbContext.CreateConnection())
            {
                connection.Open();
                var logs = await connection.QueryAsync<WatchLoggerModel>(query);
                connection.Close();
                return logs.AsList();
            }
        }

        public static async Task InsertLog(WatchLoggerModel log)
        {
            var query = @$"INSERT INTO {Constants.LogsTableName} (message,timestamp,callingFrom,callingMethod,lineNumber) " +
                "VALUES (@Message,@Timestamp,@CallingFrom,@CallingMethod,@LineNumber);";

            var parameters = new DynamicParameters();
            parameters.Add("Message", log.Message, DbType.String);
            parameters.Add("CallingFrom", log.CallingFrom, DbType.String);
            parameters.Add("CallingMethod", log.CallingMethod, DbType.String);
            parameters.Add("LineNumber", log.LineNumber, DbType.Int32);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("Timestamp", log.Timestamp.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("Timestamp", log.Timestamp, DbType.DateTime);
            }

            using (var connection = ExternalDbContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }




        public static async Task<bool> ClearLogs()
        {
            var watchlogQuery = @$"truncate table {Constants.WatchLogTableName}";
            var exQuery = @$"truncate table {Constants.WatchLogExceptionTableName}";
            var logQuery = @$"truncate table {Constants.LogsTableName}";
            using (var connection = ExternalDbContext.CreateConnection())
            {
                var watchlogs = await connection.ExecuteAsync(watchlogQuery);
                var exLogs = await connection.ExecuteAsync(exQuery);
                var logs = await connection.ExecuteAsync(logQuery);
                return watchlogs > 1 && exLogs > 1 && logs > 1;
            }
        }
    }
}
