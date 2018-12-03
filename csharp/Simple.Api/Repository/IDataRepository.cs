namespace Simple.Api.Repository
{
    public interface IDataRepository
    {
        string GetData(string key);
        bool SaveData(string key, string value);
    }
}
