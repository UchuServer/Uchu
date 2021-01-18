using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Client;

namespace Uchu.World.Client
{
    /// <summary>
    /// Temporarily caches data for when a lot of client database
    /// reads are done at once, such as when the world starts.
    /// </summary>
    public class TableCache<T> where T : class
    {
        private PropertyInfo cdClientContextProperty;
        private T[] cachedData;
        private long lastAccessTime = 0;
        private bool clearDataQueued = false;
        private int clearTimeMilliseconds = 3000;
        
        /// <summary>
        /// Creates the table cache.
        /// </summary>
        public TableCache(string tableName)
        {
            // Get the CdClient property info for fetching.
            foreach (var property in typeof(CdClientContext).GetProperties())
            {
                if (property.Name != tableName) continue;
                cdClientContextProperty = property;
                break;
            }
            
            // Throw an exception if the table name is invalid.
            if (cdClientContextProperty == null)
            {
                throw new InvalidOperationException("Table name doesn't exist: " + tableName);
            }
        }

        /// <summary>
        /// Returns the values of the table asynchronously.
        /// </summary>
        public async Task<T[]> GetValuesAsync()
        {
            // Update the last access time and fetch the data if it isn't stored.
            lastAccessTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            if (cachedData == default)
            {
                // Fetch the data.
                await using var cdContext = new CdClientContext();
                cachedData = ((DbSet<T>) cdClientContextProperty.GetValue(cdContext))?.ToArray();

                // Queue clearing the data.
                if (!clearDataQueued)
                {
                    clearDataQueued = true;
                    Task.Run(ClearQueueTimeAsync);
                }
            }

            // Return the cached data.
            return cachedData;
        }

        /// <summary>
        /// Queues clearing the cached data to save memory
        /// after a period of the data not being read from.
        /// </summary>
        private async void ClearQueueTimeAsync()
        {
            while (true)
            {
                // Wait the delay before checking to clear the table.
                var startLastAccessTime = lastAccessTime;
                await Task.Delay(clearTimeMilliseconds);

                // Clear the table if it hasn't been accessed recently.
                if (startLastAccessTime != lastAccessTime) continue;
                cachedData = default;
                clearDataQueued = false;
                GC.Collect();
                return;
            }
        }
    }
}