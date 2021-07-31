using System;
using System.Numerics;
using InfectedRose.Core;
using InfectedRose.Lvl;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Object = Uchu.World.Object;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1467_script_name__removed")]
    public class CombatChallenge : GenericActivityManager
    {
        private Player _activePlayer;

        private static readonly int[] Targets = {
            Lot.CombatChallengeTarget1,
            Lot.CombatChallengeTarget2,
            Lot.CombatChallengeTarget3,
            Lot.CombatChallengeTarget4,
            Lot.CombatChallengeTarget5,
            Lot.CombatChallengeTarget6,
            Lot.CombatChallengeTarget7,
            Lot.CombatChallengeTarget8,
            Lot.CombatChallengeTarget9,
            Lot.CombatChallengeTarget10,
        };

        private const int TotalTime = 30;

        private int _targetsDestroyed;

        private GameObject _spawnedTarget;

        private bool _active = false;
        
        private readonly Quaternion _targetRotation;

        public CombatChallenge(GameObject gameObject) : base(gameObject)
        {
            // Calculate rotation quaternion for spawned objects.
            this._targetRotation = Quaternion.Concatenate(gameObject.Transform.Rotation,
                Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float) Math.PI));
            
            // Show the initial screen when player interacts (asks player to confirm they want to join)
            Listen(gameObject.OnInteract, player =>
            {
                player.Message(new NotifyClientObjectMessage
                {
                    Associate = gameObject,
                    Name = "UI_Open",
                    ParamObj = player,
                });
            });

            // Start the activity when player presses Start button
            Listen(gameObject.OnMessageBoxRespond, (player, message) =>
            {
                var button = message.Button;
                var identifier = message.Identifier;
                // Check if player clicked the correct button
                if (!(identifier == "PlayButton" && button == 1))
                    return;
                if (this._active)
                    return;
                this._active = true;
                this.Start(player);
            });
        }

        private void Start(Player player)
        {
            // Set up activity
            this.SetupActivity(1);
            // Charge cost (1 green Imaginite)
            this.ChargeActivityCost(player);

            // Remember current player
            this._activePlayer = player;

            // Keep track of this to know which target to spawn
            this._targetsDestroyed = 0;;

            // Hide interaction icon
            this.SetNetworkVar("bInUse", true);
            // Show UI
            this.SetNetworkVar("toggle", true);
            // Tell client the game takes 30 seconds
            this.SetNetworkVar("totalTime", TotalTime);
            // Tell client the remaining time every second
            this.ActivityTimerStart("updateTime", 1, TotalTime);
            // Add player and set initial score to 0
            this.MiniGameAddPlayer(_activePlayer);
            this.SetActivityValue(player, 0, 0);


            // Start spawning targets
            this.SpawnTarget(player);
        }

        private void SpawnTarget(Player player)
        {
            // Create new target
            this._spawnedTarget = GameObject.Instantiate(new LevelObjectTemplate
            {
                Lot = Targets[Math.Min(this._targetsDestroyed / 2, Targets.Length - 1)],
                Position = this.GameObject.Transform.Position,
                Rotation = this._targetRotation,
                Scale = 1,
                LegoInfo = new LegoDataDictionary(),
            }, this.Zone);

            Object.Start(this._spawnedTarget);
            GameObject.Construct(this._spawnedTarget);

            var stats = this._spawnedTarget.GetComponent<DestroyableComponent>();

            // Whenever the target takes damage, increase score by the amount of damage dealt.
            // It also increases the score when other players damage the target.
            // This is intentional; according to the LU wiki, this is how it worked in Live as well.
            Listen(stats.OnHealthChanged, (u, i) =>
            {
                if (i >= 0) // Health gets reset to max when object dies
                    return;
                this.UpdateActivityValue(player, 0, -i);
                this.SetNetworkVar("totalDmg", this.GetActivityValue(player, 0));
            });

            // If the timer hasn't ended yet, spawn a new target when this one dies.
            Listen(stats.OnDeath, () =>
            {
                this._targetsDestroyed++;
                if (this._active)
                    this.SpawnTarget(player);
            });
        }

        public override void OnActivityTimerUpdate(string name, float timeRemaining, float timeElapsed)
        {
            // Send the client the remaining time
            if (name == "updateTime")
                this.SetNetworkVar("update_time", Convert.ToInt32(timeRemaining));
        }

        public override void OnActivityTimeDone(string name)
        {
            // Time's up
            if (name == "updateTime")
            {
                this._active = false;

                // Destroy current target
                this._spawnedTarget.GetComponent<DestructibleComponent>().SmashAsync(this.GameObject);

                // Complete achievements
                var missionInventory = this._activePlayer.GetComponent<MissionInventoryComponent>();
                var score = this.GetActivityValue(this._activePlayer, 0);
                if (score >= 25)
                    missionInventory.ScriptAsync(1449, Lot.CombatChallengeActivator);
                if (score >= 100)
                    missionInventory.ScriptAsync(1869, Lot.CombatChallengeActivator);
                if (score >= 240)
                    missionInventory.ScriptAsync(1870, Lot.CombatChallengeActivator);
                if (score >= 290)
                    missionInventory.ScriptAsync(1871, Lot.CombatChallengeActivator);

                this.StopActivity(this._activePlayer, this.GetActivityValue(this._activePlayer, 0));

                // Hide UI after 5 seconds
                Zone.Schedule(() =>
                {
                    // Hide UI
                    this.SetNetworkVar("toggle", false);
                    // Allow interaction again
                    this.SetNetworkVar("bInUse", false);
                }, 5000);
            }
        }
    }
}
