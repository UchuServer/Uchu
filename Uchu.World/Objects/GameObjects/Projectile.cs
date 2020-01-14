using System.IO;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.World.Behaviors;

namespace Uchu.World
{
    public class Projectile : Object
    {
        public long ClientObjectId { get; set; }
        
        public Lot Lot { get; set; }
        
        public GameObject Owner { get; set; }
        
        public GameObject Target { get; set; }

        public async Task Impact(byte[] data, GameObject target)
        {
            target ??= Target;
            
            var tree = new BehaviorTree(Lot);

            await tree.BuildAsync();

            var stream = new MemoryStream(data);

            var reader = new BitReader(stream);
            
            var writeStream = new MemoryStream();

            var writer = new BitWriter(writeStream);
            
            ((Player) Owner)?.SendChatMessage($"Projectile HIT [{Lot}, {tree.RootBehaviors.Count}] -> {target}");

            await tree.UseAsync(Owner, reader, writer, target);
            
            Zone.BroadcastMessage(new DoClientProjectileImpact
            {
                Associate = Owner,
                Data = writeStream.ToArray(),
                Owner = Owner,
                ProjectileId = ClientObjectId,
                Target = target
            });
        }
    }
}