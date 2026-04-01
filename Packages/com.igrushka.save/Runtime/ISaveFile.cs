namespace Igrushka.Save
{
    public interface ISaveFile
    {
        void WriteObject(ISaveObject saveObject);
        void ReadObject(ISaveObject saveObject);

        void WriteInt(int value);
        int ReadInt();


    }
}
