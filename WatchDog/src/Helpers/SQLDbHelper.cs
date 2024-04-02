using Dapper;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WatchDog.src.Data;
using WatchDog.src.Models;
using WatchDog.src.Utilities;

namespace WatchDog.src.Helpers
{
    internal static class SQLDbHelper
    {
        // WATCHLOG OPERATIONS
        public static async Task<Page<WatchLog>> GetAllWatchLogs(string searchString, string verbString, string statusCode, int pageNumber,
            string tag, string eventId, string ipAddress, DateTime? initialTimeStamp, DateTime? finalTimeStamp)
        {
            string rowQuery, pageQuery, query = string.Empty, closeQuery = string.Empty;

            if (GeneralHelper.IsPostgres())
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() OVER (ORDER BY {nameof(WatchLog.Id)} DESC) as RowNum, * FROM {Constants.WatchLogTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT * FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else if (GeneralHelper.IsMySql())
            {
                rowQuery = @$"SELECT @row_number:=@row_number+1 AS RowNum, * FROM {Constants.WatchLogTableName}, (SELECT @row_number:=0) AS t WHERE 1=1 ORDER BY {nameof(WatchLog.Id)} DESC ";
                pageQuery = @$"SELECT *, DATE_FORMAT(creation_date, '%M %d, %Y %h:%i %p') as StartTime FROM ({rowQuery}) AS data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else // SQL Server
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() over(ORDER BY {nameof(WatchLog.Id)} DESC) as RowNum, * FROM {Constants.WatchLogTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT *, FORMAT(creation_date, 'MMMM dd, yyyy hh:mm tt') as StartTime FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
         
            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(verbString) || !string.IsNullOrEmpty(statusCode) 
                || !string.IsNullOrEmpty(tag) || !string.IsNullOrEmpty(eventId) || !string.IsNullOrEmpty(ipAddress) || initialTimeStamp != null || finalTimeStamp != null)
            {
                query += "AND ( 1=1";
                closeQuery = " ) ";
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND ({nameof(WatchLog.Path)} ILIKE '%{searchString}%' OR {nameof(WatchLog.Method)} ILIKE '%{searchString}%' OR {nameof(WatchLog.ResponseStatus)}::text ILIKE '%{searchString}%' OR {nameof(WatchLog.QueryString)} ILIKE '%{searchString}%' OR {nameof(WatchLog.RequestBody)} ILIKE '%{searchString}%')";
                else
                    query += $" AND ({nameof(WatchLog.Path)} LIKE '%{searchString}%' OR {nameof(WatchLog.Method)} LIKE '%{searchString}%' OR {nameof(WatchLog.ResponseStatus)} LIKE '%{searchString}%' OR {nameof(WatchLog.QueryString)} LIKE '%{searchString}%' OR {nameof(WatchLog.RequestBody)} LIKE '%{searchString}%')";
            }

            if (!string.IsNullOrEmpty(verbString))
            {
                query += $" AND {nameof(WatchLog.Method)} LIKE '%{verbString}%' ";
            }

            if (!string.IsNullOrEmpty(statusCode))
            {
                query += $" AND {nameof(WatchLog.ResponseStatus)} = {statusCode}";
            }

            if (!string.IsNullOrEmpty(tag))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLog.Tag)} ILIKE '%{tag}%' ";
                else
                    query += $" AND {nameof(WatchLog.Tag)} LIKE '%{tag}%' ";
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLog.EventId)} ILIKE '%{eventId}%' ";
                else
                    query += $" AND {nameof(WatchLog.EventId)} LIKE '%{eventId}%' ";
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                query += $" AND {nameof(WatchLog.IpAddress)} LIKE '%{ipAddress}%' ";
            }

            if (initialTimeStamp != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLog.StartTime)} >= '{initialTimeStamp.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date >= '{initialTimeStamp.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }

            if (finalTimeStamp != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLog.StartTime)} <= '{finalTimeStamp.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date <= '{finalTimeStamp.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }
            
            query += closeQuery;
            var countQuery = @$"SELECT COUNT(*) FROM {Constants.WatchLogTableName} WHERE 1=1 {query}";
            var fullQuery = rowQuery + query + pageQuery + query;
            
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                var countData = await connection.QueryAsync<int>(countQuery);
                var logs = await connection.QueryAsync<WatchLog>(fullQuery);
                connection.Close();
                
                var count = countData.ElementAt(0);
                return logs.ToPaginatedList(pageNumber, count, Constants.PageSize);
            }
        }

        public static async Task InsertWatchLog(WatchLog log)
        {
            bool isPostgres = GeneralHelper.IsPostgres();
            var query = @$"INSERT INTO {Constants.WatchLogTableName} (responseBody,responseStatus,requestBody,queryString,path,requestHeaders,responseHeaders,method,host,ipAddress,timeSpent,startTime,endTime,tag,eventId) " +
                "VALUES (@ResponseBody,@ResponseStatus,@RequestBody,@QueryString,@Path,@RequestHeaders,@ResponseHeaders,@Method,@Host,@IpAddress,@TimeSpent,@StartTime,@EndTime,@Tag,@EventId);";

            var parameters = new DynamicParameters();
            parameters.Add("ResponseBody", isPostgres ? log.ResponseBody.Replace("\u0000", "") : log.ResponseBody, DbType.String);
            parameters.Add("ResponseStatus", log.ResponseStatus, DbType.Int32);
            parameters.Add("RequestBody", isPostgres ? log.RequestBody.Replace("\u0000", "") : log.RequestBody, DbType.String);
            parameters.Add("QueryString", log.QueryString, DbType.String);
            parameters.Add("Path", log.Path, DbType.String);
            parameters.Add("RequestHeaders", log.RequestHeaders, DbType.String);
            parameters.Add("ResponseHeaders", log.ResponseHeaders, DbType.String);
            parameters.Add("Method", log.Method, DbType.String);
            parameters.Add("Host", log.Host, DbType.String);
            parameters.Add("IpAddress", log.IpAddress, DbType.String);
            parameters.Add("TimeSpent", log.TimeSpent, DbType.String);

            if (isPostgres)
            {
                parameters.Add("StartTime", log.StartTime.ToUniversalTime(), DbType.DateTime);
                parameters.Add("EndTime", log.EndTime.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("StartTime", log.StartTime);
                parameters.Add("EndTime", log.EndTime);
            }

            parameters.Add("Tag", log.Tag, DbType.String);
            parameters.Add("EventId", log.EventId, DbType.String);

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                await connection.ExecuteAsync(query, parameters);
                connection.Close();
            }
        }



        // WATCH EXCEPTION OPERATIONS
        public static async Task<Page<WatchExceptionLog>> GetAllWatchExceptionLogs(string searchString, int pageNumber, bool negateTypeOf, 
            string typeOf, string tag, string eventId, string ipAddress, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            string rowQuery, pageQuery, query = string.Empty, closeQuery = string.Empty;
            
            if (GeneralHelper.IsPostgres())
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() OVER (ORDER BY {nameof(WatchExceptionLog.Id)} DESC) as RowNum, * FROM {Constants.WatchLogExceptionTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT * FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else if (GeneralHelper.IsMySql())
            {
                rowQuery = @$"SELECT @row_number:=@row_number+1 AS RowNum, * FROM {Constants.WatchLogExceptionTableName}, (SELECT @row_number:=0) AS t WHERE 1=1 ORDER BY {nameof(WatchExceptionLog.Id)} DESC ";
                pageQuery = @$"SELECT *, DATE_FORMAT(creation_date, '%M %d, %Y %h:%i %p') as EncounteredAt FROM ({rowQuery}) AS data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else // SQL Server
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() over(ORDER BY {nameof(WatchExceptionLog.Id)} DESC) as RowNum, * FROM {Constants.WatchLogExceptionTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT *, FORMAT(creation_date, 'MMMM dd, yyyy hh:mm tt') as EncounteredAt FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            
            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(typeOf) || !string.IsNullOrEmpty(tag) || !string.IsNullOrEmpty(eventId) || !string.IsNullOrEmpty(ipAddress) || initialEncounteredAt != null || finalEncounteredAt != null)
            {
                query += "AND ( 1=1";
                closeQuery = " ) ";
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND ({nameof(WatchExceptionLog.Source)} ILIKE '%{searchString}%' OR {nameof(WatchExceptionLog.Message)} ILIKE '%{searchString}%' OR {nameof(WatchExceptionLog.StackTrace)} ILIKE '%{searchString}%')";                                            
                else
                    query += $" AND ({nameof(WatchExceptionLog.Source)} LIKE '%{searchString}%' OR {nameof(WatchExceptionLog.Message)} LIKE '%{searchString}%' OR {nameof(WatchExceptionLog.StackTrace)} LIKE '%{searchString}%')";                                            
            }

            if (!string.IsNullOrEmpty(typeOf))
            {
                if (negateTypeOf)
                    query += $" AND {nameof(WatchExceptionLog.TypeOf)} NOT LIKE '%{typeOf}%' ";
                else
                    query += $" AND {nameof(WatchExceptionLog.TypeOf)} LIKE '%{typeOf}%' ";
            }

            if (!string.IsNullOrEmpty(tag))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchExceptionLog.Tag)} ILIKE '%{tag}%' ";
                else
                    query += $" AND {nameof(WatchExceptionLog.Tag)} LIKE '%{tag}%' ";
            }

            if (!string.IsNullOrEmpty(eventId))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchExceptionLog.EventId)} ILIKE '%{eventId}%' ";
                else
                    query += $" AND {nameof(WatchExceptionLog.EventId)} LIKE '%{eventId}%' ";
            }

            if (!string.IsNullOrEmpty(ipAddress))
            {
                query += $" AND {nameof(WatchExceptionLog.IpAddress)} LIKE '%{ipAddress}%' ";
            }

            if (initialEncounteredAt != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchExceptionLog.EncounteredAt)} >= '{initialEncounteredAt.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date >= '{initialEncounteredAt.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }

            if (finalEncounteredAt != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchExceptionLog.EncounteredAt)} <= '{finalEncounteredAt.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date <= '{finalEncounteredAt.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }

            query += closeQuery;
            var countQuery = @$"SELECT COUNT(*) FROM {Constants.WatchLogExceptionTableName} WHERE 1=1 {query}";
            var fullQuery = rowQuery + query + pageQuery + query;

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                var countData = await connection.QueryAsync<int>(countQuery);
                var logs = await connection.QueryAsync<WatchExceptionLog>(fullQuery);
                connection.Close();

                var count = countData.ElementAt(0);
                return logs.ToPaginatedList(pageNumber, count, Constants.PageSize);
            }
        }

        public static async Task InsertWatchExceptionLog(WatchExceptionLog log)
        {
            var query = @$"INSERT INTO {Constants.WatchLogExceptionTableName} (message,stackTrace,typeOf,source,path,method,queryString,requestBody,encounteredAt,host,ipAddress,timeSpent,tag,eventId) " +
                "VALUES (@Message,@StackTrace,@TypeOf,@Source,@Path,@Method,@QueryString,@RequestBody,@EncounteredAt,@Host,@IpAddress,@TimeSpent,@Tag,@EventId);";

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

            parameters.Add("Host", log.Host, DbType.String);
            parameters.Add("IpAddress", log.IpAddress, DbType.String);
            parameters.Add("TimeSpent", log.TimeSpent, DbType.String);
            parameters.Add("Tag", log.Tag, DbType.String);
            parameters.Add("EventId", log.EventId, DbType.String);

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        // LOGS OPERATION
        public static async Task<Page<WatchLoggerModel>> GetAllLogs(string searchString, string logLevelString, int pageNumber, string tag, DateTime? initialEncounteredAt, DateTime? finalEncounteredAt)
        {
            string rowQuery, pageQuery, query = string.Empty, closeQuery = string.Empty;

            if (GeneralHelper.IsPostgres())
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() OVER (ORDER BY {nameof(WatchLoggerModel.Id)} DESC) as RowNum, * FROM {Constants.LogsTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT * FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else if (GeneralHelper.IsMySql())
            {
                rowQuery = @$"SELECT @row_number:=@row_number+1 AS RowNum, * FROM {Constants.LogsTableName}, (SELECT @row_number:=0) AS t WHERE 1=1 ORDER BY {nameof(WatchLoggerModel.Id)} DESC ";
                pageQuery = @$"SELECT *, DATE_FORMAT(creation_date, '%M %d, %Y %h:%i %p') as Timestamp FROM ({rowQuery}) AS data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            else // SQL Server
            {
                rowQuery = @$"WITH data as (SELECT ROW_NUMBER() over(ORDER BY {nameof(WatchLoggerModel.Id)} DESC) as RowNum, * FROM {Constants.LogsTableName} WHERE 1=1 ";
                pageQuery = @$") SELECT *, FORMAT(creation_date, 'MMMM dd, yyyy hh:mm tt') as Timestamp FROM data WHERE RowNum > (({pageNumber}-1) * {Constants.PageSize}) AND RowNum <= ({pageNumber} * {Constants.PageSize}) ";
            }
            
            if (!string.IsNullOrEmpty(searchString) || !string.IsNullOrEmpty(logLevelString) || !string.IsNullOrEmpty(tag) || initialEncounteredAt != null || finalEncounteredAt != null)
            {
                query += "AND ( 1=1";
                closeQuery = " ) ";
            }

            if (!string.IsNullOrEmpty(searchString))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND ({nameof(WatchLoggerModel.CallingFrom)} ILIKE '%{searchString}%' OR {nameof(WatchLoggerModel.CallingMethod)} ILIKE '%{searchString}%' OR {nameof(WatchLoggerModel.Message)} ILIKE '%{searchString}%' OR {nameof(WatchLoggerModel.EventId)} ILIKE '%{searchString}%') ";
                else
                    query += $" AND ({nameof(WatchLoggerModel.CallingFrom)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.CallingMethod)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.Message)} LIKE '%{searchString}%' OR {nameof(WatchLoggerModel.EventId)} LIKE '%{searchString}%') ";
            }

            if (!string.IsNullOrEmpty(tag))
            {
                if(GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLoggerModel.Tag)} ILIKE '%{tag}%' ";
                else
                    query += $" AND {nameof(WatchLoggerModel.Tag)} LIKE '%{tag}%' ";
            }

            if (!string.IsNullOrEmpty(logLevelString))
            {
                query += $" AND {nameof(WatchLoggerModel.LogLevel)} LIKE '%{logLevelString}%' ";
            }

            if (initialEncounteredAt != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLoggerModel.Timestamp)} >= '{initialEncounteredAt.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date >= '{initialEncounteredAt.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }

            if (finalEncounteredAt != null)
            {
                if (GeneralHelper.IsPostgres())
                    query += $" AND {nameof(WatchLoggerModel.Timestamp)} <= '{finalEncounteredAt.Value.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of web server
                else
                    query += $" AND creation_date <= '{finalEncounteredAt.Value.ToString("yyyy-MM-dd HH:mm:ss")}' "; //Time of sql server
            }

            query += closeQuery;
            var countQuery = @$"SELECT COUNT(*) FROM {Constants.LogsTableName} WHERE 1=1 {query}";
            var fullQuery = rowQuery + query + pageQuery + query;

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                connection.Open();
                var countData = await connection.QueryAsync<int>(countQuery);
                var logs = await connection.QueryAsync<WatchLoggerModel>(fullQuery);
                connection.Close();
                var count = countData.ElementAt(0);
                return logs.ToPaginatedList(pageNumber, count, Constants.PageSize);
            }
        }

        public static async Task InsertLog(WatchLoggerModel log)
        {
            var query = @$"INSERT INTO {Constants.LogsTableName} (message,eventId,timestamp,callingFrom,callingMethod,lineNumber,logLevel,tag) " +
                "VALUES (@Message,@EventId,@Timestamp,@CallingFrom,@CallingMethod,@LineNumber,@LogLevel,@Tag);";

            var parameters = new DynamicParameters();
            parameters.Add("Message", log.Message, DbType.String);
            parameters.Add("CallingFrom", log.CallingFrom, DbType.String);
            parameters.Add("CallingMethod", log.CallingMethod, DbType.String);
            parameters.Add("LineNumber", log.LineNumber, DbType.Int32);
            parameters.Add("LogLevel", log.LogLevel, DbType.String);
            parameters.Add("EventId", log.EventId, DbType.String);

            if (GeneralHelper.IsPostgres())
            {
                parameters.Add("Timestamp", log.Timestamp.ToUniversalTime(), DbType.DateTime);
            }
            else
            {
                parameters.Add("Timestamp", log.Timestamp, DbType.DateTime);
            }

            parameters.Add("Tag", log.Tag, DbType.String);

            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }




        public static async Task<bool> ClearLogs()
        {
            var watchlogQuery = @$"truncate table {Constants.WatchLogTableName}";
            var exQuery = @$"truncate table {Constants.WatchLogExceptionTableName}";
            var logQuery = @$"truncate table {Constants.LogsTableName}";
            using (var connection = ExternalDbContext.CreateSQLConnection())
            {
                var watchlogs = await connection.ExecuteAsync(watchlogQuery);
                var exLogs = await connection.ExecuteAsync(exQuery);
                var logs = await connection.ExecuteAsync(logQuery);
                return watchlogs > 1 && exLogs > 1 && logs > 1;
            }
        }
    }
}
