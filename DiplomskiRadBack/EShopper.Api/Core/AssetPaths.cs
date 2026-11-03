namespace EShopper.Api.Core
{
    public static class AssetPaths
    {
        public const string FrontendFolder = "DiplomskiRadFront";

        public static string Root(string contentRootPath)
        {
            return Path.Combine(contentRootPath, "..", "..", FrontendFolder, "src", "assets");
        }
    }
}
