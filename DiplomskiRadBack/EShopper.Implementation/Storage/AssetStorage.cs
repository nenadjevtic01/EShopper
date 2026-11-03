using EShopper.Application.Storage;

namespace EShopper.Implementation.Storage
{
    public class AssetStorage : IAssetStorage
    {
        private const string Prefix = "assets/";

        private readonly string _assetsRoot;

        public AssetStorage(string assetsRoot)
        {
            _assetsRoot = Path.GetFullPath(assetsRoot);
        }

        public void Delete(string path)
        {
            var relative = (path ?? "").Split('?')[0].Trim();

            if (!relative.StartsWith(Prefix) || relative.Contains("..") || relative.Contains('\\'))
            {
                return;
            }

            var root = _assetsRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            var fullPath = Path.GetFullPath(Path.Combine(_assetsRoot, relative.Substring(Prefix.Length)));

            if (!fullPath.StartsWith(root, StringComparison.OrdinalIgnoreCase) || !File.Exists(fullPath))
            {
                return;
            }

            try
            {
                File.Delete(fullPath);
            }
            catch (IOException)
            {
            }
        }

        public void EnsureFolder(string folder)
        {
            var path = ImageFolderPath(folder);

            if (path != null)
            {
                Directory.CreateDirectory(path);
            }
        }

        public void MoveFolder(string fromFolder, string toFolder)
        {
            var source = ImageFolderPath(fromFolder);
            var target = ImageFolderPath(toFolder);

            if (source == null || target == null)
            {
                return;
            }

            if (!Directory.Exists(source))
            {
                Directory.CreateDirectory(target);
                return;
            }

            if (!Directory.Exists(target))
            {
                Directory.Move(source, target);
                return;
            }

            foreach (var file in Directory.GetFiles(source))
            {
                var destination = Path.Combine(target, Path.GetFileName(file));

                if (!File.Exists(destination))
                {
                    File.Move(file, destination);
                }
            }

            if (!Directory.EnumerateFileSystemEntries(source).Any())
            {
                Directory.Delete(source);
            }
        }

        private string? ImageFolderPath(string folder)
        {
            if (!CategoryFolder.IsValid(folder))
            {
                return null;
            }

            return Path.Combine(_assetsRoot, "img", folder);
        }
    }
}
