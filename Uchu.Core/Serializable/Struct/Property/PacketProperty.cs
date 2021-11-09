using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using InfectedRose.Core;
using InfectedRose.Lvl;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class PacketProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo StructProperty { get; }

        /// <summary>
        /// Special cases for written property types.
        /// </summary>
        public static Dictionary<Type, Action<BitWriter, object>> CustomWriters { get; }  =
            new Dictionary<Type, Action<BitWriter, object>>()
            {
                {
                    typeof(bool), (writer, o) => { writer.WriteBit((bool) o); }
                },
                {
                    typeof(LegoDataDictionary), (writer, o) =>
                    {
                        var data = (LegoDataDictionary) o;
                        if (data != null)
                        {
                            var ldf = data.ToString("\n");
                            writer.Write((uint) ldf.Length);
                            if (ldf.Length > 0)
                            {
                                writer.WriteString(ldf, ldf.Length, true);
                                writer.Write((byte) 0);
                                writer.Write((byte) 0);
                            }
                        }
                        else
                        {
                            writer.Write<uint>(0u);
                        }
                    }
                },
            };

        /// <summary>
        /// Special cases for read property types.
        /// </summary>
        public static Dictionary<Type, Func<BitReader, Dictionary<string, object>, object>> CustomReaders { get; } = new Dictionary<Type, Func<BitReader, Dictionary<string, object>, object>>()
        {
            {
                typeof(bool), (reader, context) =>
                {
                    return reader.ReadBit();
                }
            },
            {
                typeof(LegoDataDictionary), (reader, context) =>
                {
                    var length = (int) reader.Read<uint>();
                    var legoDataDictionary = LegoDataDictionary.FromString(reader.ReadString(length, true));
                    if (length > 0) reader.Read<ushort>(); // Trailing null bytes

                    return legoDataDictionary;
                }
            },
        };
        
        /// <summary>
        /// Reflection information for the Write<>(object) method in BitWriter.
        /// </summary>
        private static MethodInfo _bitWriterWriteMethod = typeof(BitWriter).GetMethods().FirstOrDefault(newMethod => newMethod.Name == "Write" && newMethod.GetParameters().Length == 1);

        /// <summary>
        /// Reflection information for the Read<>(object) method in BitReader.
        /// </summary>
        private static MethodInfo _bitReaderReadMethod = typeof(BitReader).GetMethods().FirstOrDefault(newMethod => newMethod.Name == "Read" && newMethod.GetParameters().Length == 0);
        
        /// <summary>
        /// Type of the property that is written.
        /// </summary>
        private Type _propertyType;

        /// <summary>
        /// Type of the property that the array length is written.
        /// Property is unused for non-arrays.
        /// </summary>
        private Type _arrayLengthPropertyType = typeof(uint);

        private bool _arrayNoLenght = false;

        /// <summary>
        /// Custom property writer for the type.
        /// </summary>
        private Action<BitWriter, object> _customWriter;

        /// <summary>
        /// Custom property reader for the type.
        /// </summary>
        private Func<BitReader, Dictionary<string, object>, object> _customReader;
        
        /// <summary>
        /// Creates the packet property.
        /// </summary>
        /// <param name="property">Property to write.</param>
        public PacketProperty(PropertyInfo property)
        {
            // Store the property.
            this.StructProperty = property;
            
            // Get the type to write.
            if (property == null) return;
            this._propertyType = property.PropertyType;
            if (this._propertyType.IsEnum)
            {
                this._propertyType = Enum.GetUnderlyingType(this._propertyType);
            }
            
            // Get the custom writer and reader.
            var readerWriterType = this._propertyType;
            if (readerWriterType.IsArray)
            {
                readerWriterType = readerWriterType.GetElementType();
            }
            if (readerWriterType != null)
            {
                // Set the writer and reader for types.
                foreach (var (customWriterType, customWriter) in CustomWriters)
                {
                    if (customWriterType == readerWriterType || readerWriterType.IsSubclassOf(customWriterType))
                    {
                        this._customWriter = customWriter;
                    }
                }
                foreach (var (customReaderType, customReader) in CustomReaders)
                {
                    if (customReaderType == readerWriterType || readerWriterType.IsSubclassOf(customReaderType))
                    {
                        this._customReader = customReader;
                    }
                }
                
                // Set the writer and reader for structs.
                if (readerWriterType.IsValueType && readerWriterType.GetCustomAttribute<StructAttribute>() != null)
                {
                    var structPacketProperty = new StructPacketProperty(readerWriterType);
                    this._customWriter = ((writer, o) =>
                    {
                        structPacketProperty.Write(o, writer, null);
                    });
                    this._customReader = ((reader, context) =>
                    {
                        var o = Activator.CreateInstance(readerWriterType);
                        var writtenProperties = new Dictionary<string, object>();
                        structPacketProperty.Read(o, reader, writtenProperties, context);
                        return o;
                    });
                }
            }

            // Get the array length type.
            if (property.GetCustomAttribute(typeof(StoreLengthAsAttribute)) is StoreLengthAsAttribute storeLengthAs)
            {
                this._arrayLengthPropertyType = storeLengthAs.Type;
            }

            if(property.GetCustomAttribute(typeof(NoLengthAttribute)) is NoLengthAttribute noLength)
                this._arrayNoLenght = true;
        }

        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            // Write the property.
            var value = this.StructProperty.GetValue(objectToWrite);
            if (this._propertyType.IsArray)
            {
                // Write the length if the type is defined.
                // Some packets, like UIMessageServerToSingleClientMessage, do not send a length.
                var valueArray = (Array) value;
                if (this._arrayLengthPropertyType != null && !_arrayNoLenght) {
                    var length = Convert.ChangeType(valueArray.Length, this._arrayLengthPropertyType);
                    _bitWriterWriteMethod.MakeGenericMethod(this._arrayLengthPropertyType)
                        .Invoke(writer, new object[] { length });
                }
                
                // Write the array values.
                foreach (var subValue in valueArray)
                {
                    if (_arrayNoLenght)
                        writer.WriteBit(true);

                    if (this._customWriter != null)
                    {
                        this._customWriter(writer, subValue);
                    } else {
                        _bitWriterWriteMethod.MakeGenericMethod(this._propertyType.GetElementType())
                            .Invoke(writer, new object[] { subValue });
                    }
                }

                if (_arrayNoLenght)
                    writer.WriteBit(false);
            }
            else
            {
                // Write the value.
                if (this._customWriter != null)
                {
                    this._customWriter(writer, value);
                } else {
                    _bitWriterWriteMethod.MakeGenericMethod(this._propertyType)
                        .Invoke(writer, new object[] { value });
                }
            }
            

            // Store the written property.
            if (writtenProperties != null)
            {
                writtenProperties[this.StructProperty.Name] = value;
            }
        }

        /// <summary>
        /// Reads the property to the reader.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to read.</param>
        /// <param name="reader">Bit reader to read to.</param>
        /// <param name="readProperties">Properties that were previously read.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        public void Read(object objectToWrite, BitReader reader, Dictionary<string, object> readProperties, Dictionary<string, object> context)
        {
            // Read the property.
            object value = null;
            if (this._propertyType.IsArray)
            {
                var length = Convert.ToUInt32(_bitReaderReadMethod.MakeGenericMethod(this._arrayLengthPropertyType)
                        .Invoke(reader, null));
                var valueArray = Array.CreateInstance(this._propertyType.GetElementType(), length);
                value = valueArray;
                for (var i = 0; i < length; i++) {
                    if (this._customReader != null)
                    {
                        valueArray.SetValue(this._customReader(reader, context), i);
                    }
                    else
                    {
                        valueArray.SetValue(_bitReaderReadMethod.MakeGenericMethod(this._propertyType.GetElementType())
                            .Invoke(reader, null), i);
                    }
                }
            } else {
                if (this._customReader != null)
                {
                    value = this._customReader(reader, context);
                }
                else
                {
                    value = _bitReaderReadMethod.MakeGenericMethod(this._propertyType).Invoke(reader, null);
                }
            }
            
            // Set the value.
            if (this.StructProperty.CanWrite)
            {
                this.StructProperty.SetValue(objectToWrite, value);
            }

            // Store the written property.
            if (readProperties != null)
            {
                readProperties[this.StructProperty.Name] = this.StructProperty.GetValue(objectToWrite);
            }
        }
    }
}
