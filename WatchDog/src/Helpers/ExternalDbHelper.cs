using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using WatchDog.src.Data;
using WatchDog.src.Models;
using WatchDog.src.Utilities;

namespace WatchDog.src.Helpers
{
    internal static class ExternalDbHelper
    {
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
            parameters.Add("StartTime", log.StartTime.ToString(), DbType.DateTime);
            parameters.Add("EndTime", log.EndTime.ToString(), DbType.DateTime);

            using (var connection = ExternalDbContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

       


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
            parameters.Add("EncounteredAt", log.EncounteredAt, DbType.DateTime);

            using (var connection = ExternalDbContext.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
