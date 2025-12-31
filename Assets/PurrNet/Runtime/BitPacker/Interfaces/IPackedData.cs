using System;
using PurrNet.Modules;
using PurrNet.Utils;

namespace PurrNet.Packing
{
    public class NetworkRegister
    {
        [UsedByIL]
        public static void Hash(RuntimeTypeHandle handle)
        {
            var type = Type.GetTypeFromHandle(handle);
            Hasher.PrepareType(type);
        }
    }

    public interface IPackedAuto
    {
    }

    /// <summary>
    /// Marks a type as self-serializable, meaning its serializer
    /// should not cascade into base or derived class serializers.
    /// Only this type's serializer will be used.
    /// </summary>
    public interface IStandaloneSerializable {}

    public interface IPacked
    {
        void Write(BitPacker packer);

        void Read(BitPacker packer);
    }

    public interface IPackedSimple
    {
        void Serialize(BitPacker packer);
    }
}
