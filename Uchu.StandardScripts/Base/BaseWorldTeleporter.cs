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

        private const string Identifier = "TransferBox";

        protected BaseWorldTeleporter(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, player =>
            {
                player.Message(new DisplayMessageBoxMessage
                {
                    Associate = player,
                    Identifier = Identifier,
                    CallbackClient = gameObject,
                    Show = true,
                    UserData = this.TargetZone.ToString(),
                    Text = this.MessageBoxText,
                });
            });
            Listen(this.Zone.OnPlayerLoad, player =>
            {
                this.Listen(player.OnMessageBoxRespond, (button, identifier, userData) =>
                {
                    // Ensure player clicked correct button
                    if (!(identifier == Identifier && userData == this.TargetZone.ToString()))
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
