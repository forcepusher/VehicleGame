namespace Igrushka.Save
{
    public interface ISaveable
    {
        void Save(ISaveFile saveFile);
        void Load(ISaveFile saveFile);
    }
}
