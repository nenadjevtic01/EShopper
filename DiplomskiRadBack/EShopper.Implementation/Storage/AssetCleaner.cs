using EShopper.Application.Storage;
using EShopper.DataAccess;

namespace EShopper.Implementation.Storage
{
    public class AssetCleaner
    {
        private const string ImageRoot = "assets/img/";

        private readonly EShopperDbContext _context;
        private readonly IAssetStorage _storage;

        public AssetCleaner(EShopperDbContext context, IAssetStorage storage)
        {
            _context = context;
            _storage = storage;
        }

        public static string Normalize(string src)
        {
            var value = (src ?? "").Split('?')[0].Trim();

            while (value.StartsWith("../"))
            {
                value = value.Substring(3);
            }

            return value.TrimStart('/');
        }

        public void DeleteIfUnused(string src)
        {
            var path = Normalize(src);
            var relative = path.StartsWith(ImageRoot) ? path.Substring(ImageRoot.Length) : "";

            if (!relative.Contains('/'))
            {
                return;
            }

            var used = _context.Pictures.Select(x => x.Src).ToList();
            used.AddRange(_context.Categories.Select(x => x.Src).ToList());
            used.AddRange(_context.Brands.Select(x => x.Src).ToList());
            used.AddRange(_context.HeroSlides.Select(x => x.MediaSrc).ToList());
            used.AddRange(_context.Users.Where(x => x.AvatarSrc != null).Select(x => x.AvatarSrc).ToList());

            if (used.Any(x => string.Equals(Normalize(x), path, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            _storage.Delete(path);
        }
    }
}
