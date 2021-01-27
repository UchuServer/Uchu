using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Systems.Missions;

namespace Uchu.World.Client
{
    /// <summary>
    /// Cache of the cd client context table
    /// </summary>
    public static class ClientCache
    {
        /// <summary>
        /// Cache of the table objects used by GetTable and GetTableAsync
        /// </summary>
        private static Dictionary<string,object> cacheTables = new Dictionary<string,object>();
        
        /// <summary>
        /// All missions in the cd client
        /// </summary>
        public static MissionInstance[] Missions { get; private set; } = { };

        /// <summary>
        /// All achievements in the cd client
        /// </summary>
        public static MissionInstance[] Achievements { get; private set; } = { };

        /// <summary>
        /// Loads the initial cache
        /// </summary>
        public static async Task LoadAsync()
        {
            Logger.Debug("Setting up missions cache");
            var missionTasks = GetTable<Missions>()
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
            if (!cacheTables.ContainsKey(tableName))
            {
                cacheTables[tableName] = new TableCache<T>(tableName);
            }

            return await ((TableCache<T>) cacheTables[tableName]).GetValuesAsync();
        }

        /// <summary>
        /// Fetches the values of a table.
        /// </summary>
        public static T[] GetTable<T>() where T : class
        {
            return GetTableAsync<T>().Result;
        }
    }
}