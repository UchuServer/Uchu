using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Client.Attribute;
using Uchu.World.Systems.Missions;

namespace Uchu.World.Client
{
    /// <summary>
    /// Cache of the cd client context table
    /// </summary>
    public static class ClientCache
    {
        /// <summary>
        /// Cache of the tables.
        /// </summary>
        private static Dictionary<Type,BaseTableCache> _cacheTables = new Dictionary<Type,BaseTableCache>();
        
        /// <summary>
        /// Cache of the table objects used by GetTable and GetTableAsync
        /// </summary>
        private static Dictionary<string,object> _legacyCacheTables = new Dictionary<string,object>();
        
        /// <summary>
        /// All missions in the cd client
        /// </summary>
        private static MissionInstance[] Missions { get; set; } = { };

        /// <summary>
        /// All achievements in the cd client
        /// </summary>
        public static MissionInstance[] Achievements { get; private set; } = { };

        /// <summary>
        /// Special cache table for MissionInstance since it requires the Id to be the
        /// id while the Uid is required for other cases.
        /// </summary>
        public static BurstCacheTable MissionTasksWithMissionIdCacheTable = new BurstCacheTable(typeof(MissionTasks), typeof(MissionTasks).GetProperty("Id"));

        /// <summary>
        /// Loads the initial cache
        /// </summary>
        public static async Task LoadAsync()
        {
            // Set up the cache tables.
            Logger.Debug("Setting up cache tables");
            var loadTableTasks = new List<Task>();
            foreach (var clientTableType in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => (t.GetCustomAttribute<TableAttribute>() != null)))
            {
                // Determine the index.
                var indexPropertyInfo = clientTableType.GetProperties().FirstOrDefault(propertyInfo => propertyInfo.GetCustomAttribute<CacheIndexAttribute>() != null);
                if (indexPropertyInfo == null) continue;
                
                // Get the cache type.
                Logger.Debug($"Setting up cache table for {clientTableType.Name} with index {indexPropertyInfo.Name}");
                var cacheType = CacheMethod.Entry;
                var cacheMethodAttribute = clientTableType.GetCustomAttribute<CacheMethodAttribute>();
                if (cacheMethodAttribute != null)
                {
                    cacheType = cacheMethodAttribute.Method;
                }

                // Create the cache table.
                BaseTableCache table = null;
                switch (cacheType)
                {
                    case CacheMethod.Entry:
                        table = new EntryCacheTable(clientTableType, indexPropertyInfo);
                        break;
                    case CacheMethod.Burst:
                        table = new BurstCacheTable(clientTableType, indexPropertyInfo);
                        break;
                    case CacheMethod.Persistent:
                        var newTable = new PersistentCacheTable(clientTableType, indexPropertyInfo);
                        table = newTable;
                        loadTableTasks.Add(Task.Run(newTable.LoadAsync));
                        break;
                }
                
                // Store the table.
                if (table == null) continue;
                _cacheTables.Add(clientTableType, table);
            }
            await Task.WhenAll(loadTableTasks);

            // Set up the mission cache.
            Logger.Debug("Setting up missions cache");
            var missionTasks = (await GetTableAsync<Missions>())
                .ToArray()
                .Select(async m =>
                {
                    var instance = new MissionInstance(m.Id ?? 0, default);
                    await instance.LoadAsync();
                    return instance;
                }).ToList();

            await Task.WhenAll(missionTasks);
            
            Missions = missionTasks.Select(t => t.Result).ToArray();
            Achievements = Missions.Where(m => !m.IsMission).ToArray();
        }

        /// <summary>
        /// Fetches the values of a table asynchronously.
        /// Will return without waiting if the data is already stored.
        /// </summary>
        public static async Task<T[]> GetTableAsync<T>() where T : class
        {
            var tableName = typeof(T).Name + "Table";
            if (!_legacyCacheTables.ContainsKey(tableName))
            {
                _legacyCacheTables[tableName] = new TableCache<T>(tableName);
            }

            return await ((TableCache<T>) _legacyCacheTables[tableName]).GetValuesAsync();
        }

        /// <summary>
        /// Fetches the values of a table.
        /// </summary>
        public static T[] GetTable<T>() where T : class
        {
            return GetTableAsync<T>().Result;
        }

        /// <summary>
        /// Returns the cache table for the given type.
        /// </summary>
        /// <typeparam name="T">Type of the table.</typeparam>
        /// <returns>The cache table for the given type.</returns>
        private static BaseTableCache GetCacheTable<T>() where T : class
        {
            return _cacheTables[typeof(T)];
        }

        /// <summary>
        /// Returns the first value of the given index.
        /// </summary>
        /// <typeparam name="T">Type of the table.</typeparam>
        /// <param name="index">Index to search for.</param>
        /// <returns>The first value that matches the index.</returns>
        public static async Task<T> FindAsync<T>(object index) where T : class
        {
            return await GetCacheTable<T>().FindAsync<T>(index);
        }

        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <typeparam name="T">Type of the table.</typeparam>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public static async Task<T[]> FindAllAsync<T>(object index) where T : class
        {
            return await GetCacheTable<T>().FindAllAsync<T>(index);
        }

        /// <summary>
        /// Returns the first value of the given index.
        /// </summary>
        /// <typeparam name="T">Type of the table.</typeparam>
        /// <param name="index">Index to search for.</param>
        /// <returns>The first value that matches the index.</returns>
        public static T Find<T>(object index) where T : class
        {
            return FindAsync<T>(index).Result;
        }

        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <typeparam name="T">Type of the table.</typeparam>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public static T[] FindAll<T>(object index) where T : class
        {
            return FindAllAsync<T>(index).Result;
        }
    }
}