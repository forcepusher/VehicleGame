namespace Igrushka.Save
{
    public interface ISaveFile
    {
        void WriteObject(ISaveable saveObject);
        void ReadObject(ISaveable saveObject);

        void WriteInt(int value);
        int ReadInt();

        void WriteByte(byte value);
        byte ReadByte();

        void WriteLong(long value);
        long ReadLong();


    }
}
