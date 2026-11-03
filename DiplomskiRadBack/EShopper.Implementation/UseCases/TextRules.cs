namespace EShopper.Implementation.UseCases
{
    internal static class TextRules
    {
        public static string? NullIfBlank(string? value)
        {
            var trimmed = (value ?? "").Trim();
            return trimmed.Length == 0 ? null : trimmed;
        }
    }
}
