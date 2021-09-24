using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RakDotNet.IO;

namespace Uchu.World
{
    public abstract class ReplicaComponent : Component
    {
        public static readonly int[] ComponentOrder =
        {
            108, // Possesable
            61, // ModuleAssemblyComponent
            1, // ControllablePhysicsComponent
            3, // SimplePhysicsComponent
            20, // RigidBodyPhantomPhysicsComponent
            30, // VehiclePhysicsComponent
            40, // PhantomPhysicsComponent
            7, // DestructibleComponent
            23,
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
            50,
            107,
            69
        };

        private static readonly Dictionary<ComponentId, Type> ReplicaById;

        static ReplicaComponent()
        {
            ReplicaById = new Dictionary<ComponentId, Type>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(Component))))
            {
                if (type.IsAbstract) continue;

                ComponentId id;

                if (type.IsSubclassOf(typeof(ReplicaComponent)))
                {
                    var instance = (ReplicaComponent) Activator.CreateInstance(type, true);

                    if (instance.Id == ComponentId.Invalid) continue;

                    id = instance.Id;
                }
                else
                {
                    var attribute = type.GetCustomAttribute<ServerComponentAttribute>();

                    if (attribute == null) continue;

                    id = attribute.Id;
                }

                ReplicaById.Add(id, type);
            }
        }

        /// <summary>
        ///     Id of this ReplicaComponent.
        /// </summary>
        public abstract ComponentId Id { get; }

        public static Type GetReplica(ComponentId id)
        {
            return ReplicaById.TryGetValue(id, out var type) ? type : null;
        }

        /// <summary>
        ///     The data that is only sent once to each client.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Construct(BitWriter writer);

        /// <summary>
        ///     The data that is sent every time an update accrues.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Serialize(BitWriter writer);
    }
}
