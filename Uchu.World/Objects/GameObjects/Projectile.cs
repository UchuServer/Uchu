using System.IO;
using System.Linq;
using System.Numerics;
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
        
        public Vector3 Destination { get; set; }
        
        public float MaxDistance { get; set; }
        
        public float RadiusCheck { get; set; }

        public async Task ImpactAsync(byte[] data, GameObject target)
        {
            await target.NetFavorAsync();

            var distance = Vector3.Distance(Destination, target.Transform.Position);
            
            if (distance > RadiusCheck) return;
            
            target ??= Target;
            
            var tree = new BehaviorTree(Lot);

            await tree.BuildAsync();

            await using var stream = new MemoryStream(data);

            var reader = new BitReader(stream);
            
            await using var writeStream = new MemoryStream();

            var writer = new BitWriter(writeStream);

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

        public async Task CalculateImpactAsync(GameObject target)
        {
            await target.NetFavorAsync();

            var distance = Vector3.Distance(Destination, target.Transform.Position);
            
            if (distance > RadiusCheck) return;
            
            var tree = new BehaviorTree(Lot);

            await tree.BuildAsync();

            await using var stream = new MemoryStream();

            var writer = new BitWriter(stream);

            await tree.CalculateAsync(
                Owner,
                writer,
                tree.SkillRoots.First().Key,
                Owner.GetComponent<SkillComponent>().ClaimSyncId(),
                target
            );
            
            Zone.BroadcastMessage(new DoClientProjectileImpact
            {
                Associate = Owner,
                Data = stream.ToArray(),
                Owner = Owner,
                ProjectileId = ClientObjectId,
                Target = target
            });
        }
    }
}