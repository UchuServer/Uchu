using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ComponentId Id => ComponentId.RacingControlComponent;

        public override void Construct(BitWriter writer)
        {
            this.Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            // First write scripted activity info (list of players)
            // Atm this is done in GetConstructPacket here,
            // it could be done with base.Construct but then the
            // RacingControlSerialization struct isn't actually correct
            // base.Construct(writer);
            StructPacketParser.WritePacket(this.GetConstructPacket(), writer);
        }

        public new RacingControlSerialization GetConstructPacket()
        {
            var packet = this.GetPacket<RacingControlSerialization>();
            packet.ActivityUserInfos = new ActivityUserInfo[Participants.Count];
            for (var i = 0; i < this.Participants.Count; i++)
            {
                var participant = this.Participants[i];
                var parameters = Parameters[i];
                var activityUserInfo = new ActivityUserInfo
                {
                    User = participant,
                    ActivityValue0 = parameters[0],
                    ActivityValue1 = parameters[1],
                    ActivityValue2 = parameters[2],
                    ActivityValue3 = parameters[3],
                    ActivityValue4 = parameters[4],
                    ActivityValue5 = parameters[5],
                    ActivityValue6 = parameters[6],
                    ActivityValue7 = parameters[7],
                    ActivityValue8 = parameters[8],
                    ActivityValue9 = parameters[9],
                };
                packet.ActivityUserInfos[i] = activityUserInfo;
            }

            packet.ExpectedPlayerCount = 2; // TODO

            // TODO

            return packet;
        }
    }
}
