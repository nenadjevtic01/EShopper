namespace EShopper.Application.Storage
{
    public interface IAssetStorage
    {
        void Delete(string path);

        void EnsureFolder(string folder);

        void MoveFolder(string fromFolder, string toFolder);
    }
}
