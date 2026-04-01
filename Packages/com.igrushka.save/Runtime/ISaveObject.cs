namespace Igrushka.Save
{
    public interface ISaveObject
    {
        void Save(ISaveFile saveFile);
        void Load(ISaveFile saveFile);
    }
}
