using System.Text.RegularExpressions;

namespace EShopper.Application.Storage
{
    public static class CategoryFolder
    {
        private static readonly string[] Reserved = { "avatars", "brands", "categories", "hero" };

        public static string CleanName(string? name)
        {
            return Regex.Replace((name ?? "").Trim(), @"\s+", " ");
        }

        public static string FromName(string? name)
        {
            return CleanName(name).ToLowerInvariant().Replace(' ', '-');
        }

        public static bool IsReserved(string? name)
        {
            return Reserved.Contains(FromName(name));
        }

        public static bool IsValid(string? folder)
        {
            return Regex.IsMatch(folder ?? "", @"^[a-z]+(-[a-z]+)*$");
        }

        public static string MoveSrc(string src, string fromFolder, string toFolder)
        {
            var marker = $"assets/img/{fromFolder}/";
            var index = (src ?? "").IndexOf(marker, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
            {
                return src ?? "";
            }

            return src.Substring(0, index) + $"assets/img/{toFolder}/" + src.Substring(index + marker.Length);
        }
    }
}
