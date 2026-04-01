namespace Igrushka.Save
{
    public interface ISaveFile
    {
        void WriteObject(ISaveable saveObject);
        void ReadObject(ISaveable saveObject);

        void WriteBoolean(bool value);
        bool ReadBoolean();

        void WriteByte(byte value);
        byte ReadByte();

        void WriteInt(int value);
        int ReadInt();

        void WriteLong(long value);
        long ReadLong();

        void WriteFloat(float value);
        float ReadFloat();

        void WriteDouble(double value);
        float ReadDouble();

        void WriteString(string value);
        string ReadString();
    }
}
