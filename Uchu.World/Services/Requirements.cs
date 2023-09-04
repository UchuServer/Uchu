using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Services
{
    public static class Requirements
    {
        private static bool Check(bool a, bool b, Mode mode)
        {
            return mode switch
            {
                Mode.And => (a && b),
                Mode.Or => (a || b),
                Mode.None => false,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }

        private enum Mode
        {
            None,
            And,
            Or
        }

        private static async Task<bool> CheckAsync(string cur, Player player)
        {
            if (string.IsNullOrWhiteSpace(cur)) return true;

            return await CheckPreconditionAsync(int.Parse(cur), player);
        }

        public static async Task<bool> CheckRequirementsAsync(string requirements, Player player)
        {
            if (string.IsNullOrWhiteSpace(requirements)) return true;

            var cur = new StringBuilder();
            var res = true;
            var mode = Mode.And;

            for (var i = 0; i < requirements.Length; i++)
            {
                var chr = requirements[i];

                switch (chr)
                {
                    case ' ':
                        break;

                    case '&':
                    case ',':
                    case ';':
                        {
                            res = Check(res, await CheckAsync(cur.ToString(), player), mode);

                            cur.Clear();

                            if (!res)
                                return false;

                            mode = Mode.And;
                            break;
                        }

                    case '|':
                        {
                            res = Check(res, await CheckAsync(cur.ToString(), player), mode);

                            cur.Clear();

                            mode = Mode.Or;
                            break;
                        }

                    case '(':
                        res = Check(res, await CheckRequirementsAsync(requirements.Substring(i + 1), player), mode);
                        break;

                    case ')':
                        return Check(res, await CheckAsync(cur.ToString(), player), mode);

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case ':':
                        cur.Append(chr);
                        break;
                }
            }

            res = Check(res, await CheckAsync(cur.ToString(), player), mode);

            return res;
        }

        public static async Task<bool> CheckPreconditionAsync(int id, Player player)
        {
            var precondition = await ClientCache.FindAsync<Preconditions>(id);
            if (precondition?.Type == default)
            {
                Logger.Error($"Invalid precondition: {id}!");

                return false;
            }

            var type = (PreconditionType)precondition.Type;
            Logger.Debug($"Checking {type}");

            switch (type)
            {
                case PreconditionType.ItemEquipped:
                    return ItemEquipped(precondition, player);
                case PreconditionType.ItemNotEquipped:
                    return ItemNotEquipped(precondition, player);
                case PreconditionType.HasItem:
                    return HasItem(precondition, player);
                case PreconditionType.DoesNotHaveItem:
                    return DoesNotHaveItem(precondition, player);
                case PreconditionType.HasAchievement:
                    return HasAchievement(precondition, player);
                case PreconditionType.MissionAvailable:
                    return MissionAvailable(precondition, player);
                case PreconditionType.OnMission:
                    return OnMission(precondition, player);
                case PreconditionType.MissionComplete:
                    return MissionComplete(precondition, player);
                case PreconditionType.PetDeployed:
                    break;
                case PreconditionType.HasFlag:
                    return HasFlag(precondition, player);
                case PreconditionType.WithinShape:
                    return WithinShape(precondition, player);
                case PreconditionType.InBuild:
                    return InBuild(precondition, player);
                case PreconditionType.TeamCheck:
                    break;
                case PreconditionType.IsPetTaming:
                    break;
                case PreconditionType.HasFaction:
                    return HasFaction(precondition, player);
                case PreconditionType.DoesNotHaveFaction:
                    return DoesNotHaveFaction(precondition, player);
                case PreconditionType.HasRacingLicence:
                    return HasRacingLicence(precondition, player);
                case PreconditionType.DoesNotHaveRacingLicence:
                    return DoesNotHaveRacingLicence(precondition, player);
                case PreconditionType.LegoClubMember:
                    return LegoClubMember(precondition, player);
                case PreconditionType.NoInteraction:
                    break;
                case PreconditionType.PlayerHasLevel:
                    return PlayerHasLevel(precondition, player);
                default:
                    throw new ArgumentOutOfRangeException("type", type, "");
            }

            return false;
        }

        private static bool ItemEquipped(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryComponent>();
            return preconditions.TargetLOT.InterpretCollection().Any(lot => inventory.HasEquipped(lot));
        }

        private static bool ItemNotEquipped(Preconditions preconditions, Player player)
            => !ItemEquipped(preconditions, player);

        private static bool HasItem(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();

            var lot = preconditions.TargetLOT.InterpretCollection().First();

            var count = preconditions.TargetCount ?? 1;

            return inventory.Items.Where(i => i.Lot == lot).Sum(i => i.Count) >= count;
        }

        private static bool DoesNotHaveItem(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();

            var lot = preconditions.TargetLOT.InterpretCollection().First();

            return inventory.Items.All(i => i.Lot != lot);
        }

        private static bool HasAchievement(Preconditions preconditions, Player player)
            => MissionComplete(preconditions, player);

        private static bool MissionAvailable(Preconditions preconditions, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>();
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missions.HasAvailable(id);
        }

        private static bool OnMission(Preconditions preconditions, Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missionInventory.HasActive(id);
        }

        private static bool MissionComplete(Preconditions preconditions, Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();
            if (missionInventory == null) {
                // This happens because MissionInventoryComponent is initialized after InventoryComponent
                // when the player is initialized.
                Logger.Warning("MissionInventoryComponent is null");
                return true;
            }

            var id = preconditions.TargetLOT.InterpretCollection().First();

            return missionInventory.HasCompleted(id);
        }

        private static bool HasFlag(Preconditions preconditions, Player player)
        {
            var character = player.GetComponent<CharacterComponent>();
            return character.GetFlag(int.Parse(preconditions.TargetLOT));
        }

        private static bool WithinShape(Preconditions preconditions, Player player)
        {
            // TODO: Find a better implementation
            foreach (Lot lot in preconditions.TargetLOT.InterpretCollection())
            {
                var shape = player.Zone.GameObjects.FirstOrDefault(g => g.Lot == lot, null);
                if (shape == null)
                    continue;

                var distance = Vector3.DistanceSquared(shape.Transform.Position, player.Transform.Position);

                Logger.Debug($"Distance (squared) to {shape} is {distance}");
                if (distance <= 16 * 16)
                    return true;
            }

            return false;
        }

        private static bool InBuild(Preconditions preconditions, Player player)
        {
            var modularBuilder = player.GetComponent<ModularBuilderComponent>();
            return modularBuilder.IsBuilding;
        }

        private static bool HasFaction(Preconditions preconditions, Player player)
        {
            var character = player.GetComponent<CharacterComponent>();
            return character.IsSentinel || character.IsAssembly || character.IsParadox || character.IsVentureLeague;
        }

        private static bool DoesNotHaveFaction(Preconditions preconditions, Player player)
            => !HasFaction(preconditions, player);

        private static bool HasRacingLicence(Preconditions preconditions, Player player)
        {
            var missionInventory = player.GetComponent<MissionInventoryComponent>();
            if (missionInventory == null) {
                // This happens because MissionInventoryComponent is initialized after InventoryComponent
                // when the player is initialized.
                Logger.Warning("MissionInventoryComponent is null");
                return true;
            }

            return missionInventory.HasCompleted(623);
        }

        private static bool DoesNotHaveRacingLicence(Preconditions preconditions, Player player)
            => !HasRacingLicence(preconditions, player);

        private static bool LegoClubMember(Preconditions preconditions, Player player) {
            // TODO: Implement for real. Currently it seems this is hard coded everywhere
            return true;
        }

        private static bool PlayerHasLevel(Preconditions preconditions, Player player)
        {
            var character = player.GetComponent<CharacterComponent>();
            return character.Level >= int.Parse(preconditions.TargetLOT);
        }
    }
}