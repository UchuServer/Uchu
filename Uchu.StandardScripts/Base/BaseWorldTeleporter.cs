using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.Base
{
    public class BaseWorldTeleporter : ObjectScript
    {
        protected virtual string MessageBoxText { get; }
        protected virtual int TargetZone { get; }
        protected virtual string TargetSpawnLocation { get; }
        protected virtual string Animation { get; }
        protected virtual string AcceptIdentifier { get; } = "TransferBox";
        protected virtual string CancelIdentifier { get; } = "TransferBox";
        protected virtual bool CheckUserData { get; } = true;

        protected virtual void ShowTransferPopup(Player player)
        {
            player.Message(new DisplayMessageBoxMessage
            {
                Associate = player,
                Identifier = this.AcceptIdentifier,
                CallbackClient = GameObject,
                Show = true,
                UserData = this.TargetZone.ToString(),
                Text = this.MessageBoxText,
            });
        }

        protected BaseWorldTeleporter(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, this.ShowTransferPopup);
            
            Listen(this.Zone.OnPlayerLoad, player =>
            {
                this.Listen(player.OnMessageBoxRespond, (button, identifier, userData) =>
                {
                    // Ensure player is answering the right message box
                    // Identifier differs for accept/cancel for LEGO® Club world teleporter interaction
                    if ((identifier != this.AcceptIdentifier && identifier != this.CancelIdentifier)
                        || (CheckUserData && userData != this.TargetZone.ToString()))
                        return;
                    
                    // If user clicked no, terminate interaction
                    if (button != 1)
                    {
                        player.Message(new TerminateInteractionMessage
                        {
                            Associate = player,
                            Terminator = gameObject,
                            Type = TerminateType.FromInteraction,
                        });
                        return;
                    }

                    // User clicked yes
                    // Stun user and transfer to zone
                    player.Message(new SetStunnedMessage
                    {
                        Associate = player,
                        CantMove = true,
                        CantAttack = true,
                        CantInteract = true,
                        CantTurn = true,
                    });
                    player.Animate(this.Animation);
                    if (this.TargetSpawnLocation != null)
                        player.GetComponent<CharacterComponent>().SpawnLocationName = this.TargetSpawnLocation;

                    // Wait a bit while animation shows. After 3 seconds, show zone summary.
                    // Transfer when that's complete (user clicked Next).
                    // Ideally we'd ensure an instance is available or start one _before_ this delay.
                    this.Zone.Schedule(() =>
                    {
                        Listen(player.OnFireServerEvent, (arguments, _) =>
                        {
                            if (arguments == "summaryComplete")
                                player.SendToWorldAsync((ZoneId) this.TargetZone);
                        });
                        player.Message(new DisplayZoneSummaryMessage
                        {
                            Associate = player,
                            Sender = gameObject,
                        });
                    }, 3000);

                    // Green beam animation for NT<->AM transfer.
                    if (this.Animation == "nexus-teleport")
                    {
                        this.Zone.BroadcastMessage(new PlayFXEffectMessage
                        {
                            Associate = player,
                            EffectId = 6478,
                            EffectType = "teleportBeam",
                            Name = "crux_teleport_beam",
                        });

                        this.Zone.Schedule(() =>
                        {
                            this.Zone.BroadcastMessage(new StopFXEffectMessage
                            {
                                Associate = player,
                                Name = "crux_teleport_beam",
                            });
                        }, 2000);
                    }
                });
            });
        }
    }
}
