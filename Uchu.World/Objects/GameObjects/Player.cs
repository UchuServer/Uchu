using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World
{
    public class Player : GameObject
    {
        public IPEndPoint EndPoint { get; private set; }

        public static Player Create(Character character, IPEndPoint endPoint, Zone zone)
        {
            var instance = Instantiate<Player>(
                zone,
                character.Name,
                zone.ZoneInfo.SpawnPosition,
                zone.ZoneInfo.SpawnRotation,
                character.CharacterId,
                1
            );

            instance.EndPoint = endPoint;

            zone.RequestConstruction(instance);

            var controllablePhysics = instance.AddComponent<ControllablePhysicsComponent>();

            controllablePhysics.HasPosition = true;

            instance.AddComponent<DestructibleComponent>();

            var stats = instance.GetComponent<StatsComponent>();

            stats.HasStats = true;
            stats.CurrentHealth = (uint) character.CurrentHealth;
            stats.CurrentArmor = (uint) character.CurrentArmor;
            stats.CurrentImagination = (uint) character.CurrentImagination;
            stats.MaxHealth = (uint) character.MaximumHealth;
            stats.MaxArmor = (uint) character.MaximumArmor;
            stats.MaxImagination = (uint) character.MaximumImagination;

            var characterComponent = instance.AddComponent<CharacterComponent>();

            characterComponent.Level = (uint) character.Level;
            characterComponent.Character = character;

            var inventory = instance.AddComponent<InventoryComponent>();

            inventory.Items = character.Items.Where(i => i.IsEquipped).ToList();

            instance.AddComponent<LuaScriptComponent>();
            instance.AddComponent<SkillComponent>();
            instance.AddComponent<RendererComponent>();
            instance.AddComponent<Component107>();

            instance.Construct();

            instance.AddComponent<QuestInventory>();
            instance.AddComponent<ItemInventory>();
            instance.AddComponent<TeamPlayer>();
            
            Logger.Debug($"Player \"{character.Name}\" has been constructed.");
            
            return instance;
        }

        public void Message(IGameMessage gameMessage)
        {
            Logger.Debug($"Send {gameMessage} to {EndPoint} from {gameMessage.Associate.ObjectId}");
            Server.Send(gameMessage, EndPoint);
        }
    }
}