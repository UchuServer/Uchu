using System;
using System.IO;
using RakDotNet.IO;

namespace Uchu.World.Collections
{
    public struct ActionMessageEntry
    {
        public ActionMessageType Type { get; set; }

        public object Value { get; set; }

        public ActionMessageEntry(ActionMessageType type, object value = null)
        {
            Type = type;

            Value = value;
        }

        public void Serialize(BitWriter writer)
        {
            writer.Write((byte) Type);

            switch (Type)
            {
                case ActionMessageType.Undefined:
                    break;
                case ActionMessageType.Null:
                    break;
                case ActionMessageType.False:
                    break;
                case ActionMessageType.True:
                    break;
                case ActionMessageType.Integer:
                    WriteInt(writer, uint.Parse(Value.ToString()));
                    break;
                case ActionMessageType.Double:
                    break;
                case ActionMessageType.String:
                    break;
                case ActionMessageType.Xml:
                    break;
                case ActionMessageType.Date:
                    break;
                case ActionMessageType.Array:
                    break;
                case ActionMessageType.Object:
                    break;
                case ActionMessageType.XmlEnd:
                    break;
                case ActionMessageType.ByteArray:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void WriteInt(BitWriter writer, uint data)
        {
            if (data <= 0x7F)
            {
                writer.Write((byte) data);
            }
            else if (data <= 0x3FFF)
            {
                writer.Write((byte) ((data >> 7) | 0x80));
                writer.Write((byte) (data & 0x7F));
            }
            else if (data <= 0x001FFFFF)
            {
                writer.Write((byte) ((data >> 14) | 0x80));
                writer.Write((byte) (((data >> 7) & 0x7F) | 0x80));
                writer.Write((byte) (data & 0x7F));
            }
            else
            {
                writer.Write((byte) ((data >> 22) | 0x80));
                writer.Write((byte) (((data >> 15) & 0x7F) | 0x80));
                writer.Write((byte) (((data >> 8) & 0x7F) | 0x80));
                writer.Write((byte) (data & 0xFF));
            }
        }
    }
}