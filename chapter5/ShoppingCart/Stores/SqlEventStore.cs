using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Npgsql;
using ShoppingCart.Domains;

namespace ShoppingCart.Stores
{
    public class SqlEventStore : IEventStore
    {
        private readonly string _connectionString;
        
        private const string readEventsSql = @"
SELECT event_store_id AS SequenceNumber,
       name AS Name,
       occurred_at AS OccurredAt,
       content AS Content
  FROM shopcart.event_store
 WHERE event_store_id >= @Start and event_store_id <= @End
";
        
        private const string writeEventSql = @"
INSERT INTO shopcart.event_store(name, occurred_at, content)
VALUES (@Name, @OccurredAt, @Content)
";

        public SqlEventStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            using (var conn = await GetOpenConnectionAsync())
            {
                // Reads EventStore table rows between start and end
                var results = await conn.QueryAsync<dynamic>(
                        readEventsSql,
                        new {Start = firstEventSequenceNumber, End = lastEventSequenceNumber}
                    )
                    .ConfigureAwait(false);

                return results.Select(
                    row =>
                    {
                        var converter = new ExpandoObjectConverter();
                        var content = JsonConvert.DeserializeObject<ExpandoObject>(row.Content, converter);
                        
                        // Maps EventStore table rows to Event objects
                        return new Event(row.SequenceNumber, row.OccurredAt, row.Name, content);
                    }
                );
            }
        }

        public async Task Raise(string eventName, object content)
        {
            var jsonContent = JsonConvert.SerializeObject(content);
            using (var conn = await GetOpenConnectionAsync().ConfigureAwait(false))
            {
                // Uses Dapper to execute a simple SQL insert statement
                await conn.ExecuteAsync(
                        writeEventSql,
                        new
                        {
                            Name = eventName,
                            OccurredAt = DateTimeOffset.UtcNow,
                            Content = jsonContent
                        }
                    )
                    .ConfigureAwait(false);
            }
        }

        private async Task<IDbConnection> GetOpenConnectionAsync()
        {
            var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync().ConfigureAwait(false);

            return connection;
        }
    }
}