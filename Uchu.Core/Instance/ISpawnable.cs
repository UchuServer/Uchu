using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Uchu.Core.Scripting.Lua;

namespace Uchu.Core
{
    public interface ISpawnable
    {
        int LOT { get; }
        long ObjectId { get; }
        LuaGameObject LuaObject { get; }

        Task SpawnAsync();

        void Despawn(IPEndPoint[] endpoints = null);

        void Update(IEnumerable<IReplicaComponent> components, IPEndPoint[] endpoints = null);

        void Update(IReplicaComponent component, IPEndPoint[] endpoints = null);

        void Die(PlayerObject killer, KillType type = KillType.Violent);

        Task DropLootAsync(PlayerObject owner);
    }
}