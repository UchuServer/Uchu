using System.Numerics;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Parsers;

namespace Uchu.World
{
    using ClientComponent = Core.CdClient.RebuildComponent;
    
    [RequireComponent(typeof(StatsComponent), true)]
    public class RebuildComponent : ScriptedActivityComponent
    {
        private ClientComponent _clientComponent;

        private float _completeTime;
        private int _imaginationCost;
        private float _timeToSmash;
        private float _resetTime;
        
        public RebuildState State { get; set; } = RebuildState.Open;

        public bool Success { get; set; }

        public bool Enabled { get; set; }

        public float TimeSinceStart { get; set; }

        public float PauseTime { get; set; }

        public Vector3 ActivatorPosition { get; set; }

        public override ReplicaComponentsId Id => ReplicaComponentsId.Rebuild;

        public RebuildComponent()
        {
            OnStart += async () =>
            {
                using var cdClient = new CdClientContext();

                _clientComponent = await cdClient.RebuildComponentTable.FirstOrDefaultAsync(
                    r => r.Id == GameObject.Lot.GetComponentId(ReplicaComponentsId.Rebuild)
                );

                if (_clientComponent == default)
                {
                    Logger.Error(
                        $"{GameObject} does not have a valid {nameof(ReplicaComponentsId.Rebuild)} component."
                    );
                    
                    return;
                }

                _completeTime = _clientComponent.Completetime ?? 0;
                _imaginationCost = _clientComponent.Takeimagination ?? 0;
                _timeToSmash = _clientComponent.Timebeforesmash ?? 0;
                _resetTime = _clientComponent.Resettime ?? 0;
            };
        }
        
        public override void FromLevelObject(LevelObject levelObject)
        {
            ActivatorPosition = (Vector3) levelObject.Settings["rebuild_activators"];
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);

            writer.WriteBit(false);

            writer.Write(ActivatorPosition);

            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);

            writer.WriteBit(true);

            writer.Write((uint) State);

            writer.WriteBit(Success);
            writer.WriteBit(Enabled);

            writer.Write(TimeSinceStart);
            writer.Write(PauseTime);
        }
        
        /*
         * Rebuildables have five states.
         * 
         * Open: The Quickbuild is available and ready to be built.
         * Complete: The Quickbuild can not be built, does not mean it can not be used.
         * Resetting: This has to be sent to the client once, but does not have to be on the object for any amount of time.
         * Building: The Quickbuild is being built.
         * Incomplete: The Quickbuild is not complete but can be restarted.
         *
         * Open -> Building     ->     Complete -> Resetting -> Open
         *                \\          /           /
         *                  Incomplete     ->    /
         * 
         * All of the changes in the state of the quickbuild has to be notified to the player building and updated
         * in the world.
         *
         * NOTE: Rebuildables in AG are weird.
         */
        
        
    }
}