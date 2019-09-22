using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Behaviors
{
    public class TacArc : Behavior
    {
        public bool Hit { get; private set; }

        public GameObject[] Targets { get; private set; }

        public override BehaviorTemplateId Id => BehaviorTemplateId.TacArc;

        public override async Task Serialize(BitReader reader)
        {
            Hit = reader.ReadBit();

            if (Hit)
            {
                var param = await GetParameter(BehaviorId, "check_env");

                if (param.Value > 0)
                    reader.ReadBit();

                Targets = new GameObject[reader.Read<uint>()];

                for (var i = 0; i < Targets.Length; i++)
                {
                    Targets[i] = reader.ReadGameObject(GameObject.Zone);

                    Logger.Debug($"{GameObject} hit {Targets[i]}");

                    if (!Executioner.Targets.Contains(Targets[i]))
                        Executioner.Targets.Add(Targets[i]);

                    // TODO: Damage
                    Targets[i].GetComponent<DestructibleComponent>().Smash(GameObject, GameObject);
                }

                var action = await GetParameter(BehaviorId, "action");
                if (action.Value != null)
                    for (var i = 0; i < Targets.Length; i++)
                        await StartBranch((int) action.Value, reader);
            }
        }
    }
}