using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class PetHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task PetTamingTryBuildHandler(PetTamingTryBuildMessage message, Player player)
        {
            await player.OnPetTamingTryBuild.InvokeAsync(message);
        }

        [PacketHandler]
        public async Task ClientExitTamingMinigameHandler(ClientExitTamingMinigameMessage message, Player player)
        {
            NotifyPetTamingMinigame msg = new NotifyPetTamingMinigame();
            msg.notifyType = NotifyType.QUIT;
            msg.PetID = (ObjectId)(ulong)0;
            msg.PlayerTamingID = player.Id;
            msg.bForceTeleport = false;
            msg.petsDestPos = Vector3.Zero;
            msg.teleRot = Quaternion.Identity;
            msg.telePos = Vector3.Zero;
            msg.Associate = player;
            player.Message(msg);
        }
    }
}