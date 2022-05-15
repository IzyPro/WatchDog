using System.IO;
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
            return !string.IsNullOrEmpty(WatchDogExternalDbConfig.ConnectionString) && WatchDogSqlDriverOption.SqlDriverOption == Enums.WatchDogSqlDriverEnum.PostgreSql;
        }
    }
}
