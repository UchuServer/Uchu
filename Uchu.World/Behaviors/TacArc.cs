using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class TacArc : Behavior
    {
        public bool Hit { get; private set; }
        
        public bool EnvironmentCheck { get; private set; }

        public GameObject[] Targets { get; private set; }

        public override BehaviorTemplateId Id => BehaviorTemplateId.TacArc;

        public override async Task SerializeAsync(BitReader reader)
        {
            Hit = reader.ReadBit();

            Logger.Debug($"TacArc.Hit = {Hit}");
            
            if (Hit)
            {
                var param = await GetParameter(BehaviorId, "check_env");

                if (param.Value > 0)
                    EnvironmentCheck = reader.ReadBit();
                
                Logger.Debug($"TacArc.EnvironmentCheck = {EnvironmentCheck}");

                Targets = new GameObject[reader.Read<uint>()];

                for (var i = 0; i < Targets.Length; i++)
                {
                    var target = reader.ReadGameObject(GameObject.Zone);
                    
                    if (target == default) continue;

                    Logger.Debug($"{GameObject} hit {target}");

                    if (!Executioner.Targets.Contains(target))
                        Executioner.Targets.Add(target);

                    // TODO: Damage
                    target.GetComponent<DestructibleComponent>().Smash(GameObject);

                    Targets[i] = target;
                }

                var action = await GetParameter(BehaviorId, "action");
                if (action.Value != null)
                    for (var i = 0; i < Targets.Length; i++)
                        await StartBranch((int) action.Value, reader);
            }
        }
    }
}