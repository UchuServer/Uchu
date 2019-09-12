using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public abstract class ReplicaComponent : Component
    {
        public static readonly int[] ComponentOrder =
        {
            108,
            61,
            1,
            3,
            20,
            30,
            40,
            7,
            25,
            26,
            4,
            19,
            17,
            5,
            9,
            60,
            48,
            25,
            49,
            16,
            6,
            39,
            71,
            75,
            42,
            2,
            107,
            69
        };

        private static readonly Dictionary<ReplicaComponentsId, Type> ReplicaById;

        static ReplicaComponent()
        {
            ReplicaById = new Dictionary<ReplicaComponentsId, Type>();
            
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(ReplicaComponent))))
            {
                var instance = (ReplicaComponent) Activator.CreateInstance(type);
                
                if (instance.Id == ReplicaComponentsId.Invalid) continue;
                
                Logger.Debug($"[{instance.Id}] {type}");
                
                ReplicaById.Add(instance.Id, type);
            }
        }

        public static Type GetReplica(ReplicaComponentsId id)
        {
            return ReplicaById.TryGetValue(id, out var type) ? type : null;
        }
        
        /// <summary>
        /// Id of this ReplicaComponent.
        /// </summary>
        public abstract ReplicaComponentsId Id { get; }

        public abstract void FromLevelObject(LevelObject levelObject);
        
        /// <summary>
        /// The data that is only sent once to each client.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Construct(BitWriter writer);

        /// <summary>
        /// The data that is sent every time an update accrues.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Serialize(BitWriter writer);
    }
}