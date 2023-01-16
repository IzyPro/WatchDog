using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text.Json;
using WatchDog.src.Enums;
using WatchDog.src.Models;

namespace WatchDog.src.Helpers
{
    internal static class GeneralHelper
    {
        public static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk,
                                                   0,
                                                   readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
        }

        public static bool IsPostgres()
        {
            return !string.IsNullOrEmpty(WatchDogExternalDbConfig.ConnectionString) && WatchDogDatabaseDriverOption.DatabaseDriverOption == Enums.WatchDogDbDriverEnum.PostgreSql;
        }

        public static dynamic CamelCaseSerializer
            => WatchDog.Serializer switch
            {
                WatchDogSerializerEnum.Newtonsoft => new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                },
                _ => new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            };
        public static MemoryCacheEntryOptions cacheEntryOptions {
            get 
            {
                return new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetPriority(CacheItemPriority.High);
            }
        }
    }
}
