using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Services
{
    public static class Requirements
    {
        public static async Task<bool> CheckPreconditionAsync(int id, Player player)
        {
            await using var cdClient = new CdClientContext();

            var precondition = await cdClient.PreconditionsTable.FirstOrDefaultAsync(
                p => p.Id == id
            );

            if (precondition?.Type == default)
            {
                Logger.Error($"Invalid precondition: {id}!");

                return false;
            }

            var type = (PreconditionType) precondition.Type;

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
                    return await HasAchievementAsync(precondition, player);
                case PreconditionType.MissionAvailable:
                    return await MissionAvailableAsync(precondition, player);
                case PreconditionType.OnMission:
                    return await OnMissionAsync(precondition, player);
                case PreconditionType.MissionComplete:
                    return await MissionCompleteAsync(precondition, player);
                case PreconditionType.PetDeployed:
                    break;
                case PreconditionType.HasFlag:
                    break;
                case PreconditionType.WithinShape:
                    break;
                case PreconditionType.InBuild:
                    break;
                case PreconditionType.TeamCheck:
                    break;
                case PreconditionType.IsPetTaming:
                    break;
                case PreconditionType.HasFaction:
                    break;
                case PreconditionType.DoesNotHaveFaction:
                    break;
                case PreconditionType.HasRacingLicence:
                    break;
                case PreconditionType.DoesNotHaveRacingLicence:
                    break;
                case PreconditionType.LegoClubMember:
                    break;
                case PreconditionType.NoInteraction:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private static bool ItemEquipped(Preconditions preconditions, Player player)
        {
            var inventory = player.GetComponent<InventoryComponent>();

            var lot = preconditions.TargetLOT.InterpretCollection().First();

            return inventory.HasEquipped(lot);
        }

        private static bool ItemNotEquipped(Preconditions preconditions, Player player)
        {
            return !ItemEquipped(preconditions, player);
        }

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

        private static async Task<bool> HasAchievementAsync(Preconditions preconditions, Player player)
        {
            return await MissionCompleteAsync(preconditions, player);
        }

        private static async Task<bool> MissionAvailableAsync(Preconditions preconditions, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>();
            
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return await missions.CanAccept(id);
        }

        private static async Task<bool> OnMissionAsync(Preconditions preconditions, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>();
            
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return await missions.OnMission(id);
        }

        private static async Task<bool> MissionCompleteAsync(Preconditions preconditions, Player player)
        {
            var missions = player.GetComponent<MissionInventoryComponent>();
            
            var id = preconditions.TargetLOT.InterpretCollection().First();

            return await missions.HasCompleted(id);
        }
    }
}