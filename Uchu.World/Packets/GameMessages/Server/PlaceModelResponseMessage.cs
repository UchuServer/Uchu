using System;
using System.Numerics;
using InfectedRose.Core;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PlaceModelResponseMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlaceModelResponse;

        public Vector3 position = Vector3.Zero;
        public long propertyPlaqueID = 0;
        public int response = 0;
        public Quaternion rotation = Quaternion.Identity;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(position != Vector3.Zero);
            if (position != Vector3.Zero)
            {
                writer.Write(position.X);
                writer.Write(position.Y);
                writer.Write(position.Z);
            }

            writer.WriteBit(propertyPlaqueID != 0);
            if (propertyPlaqueID != 0)
            {
                writer.Write(propertyPlaqueID);
            }

            writer.WriteBit(response != 0);
            if (response != 0)
            {
                writer.Write(response);
            }

            writer.WriteBit(rotation != Quaternion.Identity);
            if (rotation != Quaternion.Identity) writer.WriteNiQuaternion(rotation);
        }
    }
}