using RakDotNet.IO;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Uchu.World
{
    public class NotifyTamingBuildSuccessMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.NotifyTamingBuildSuccess;

        public Vector3 BuildPosition;

        public override void Deserialize(BitReader reader)
        {
            BuildPosition = reader.Read<Vector3>();
        }
    }
}
