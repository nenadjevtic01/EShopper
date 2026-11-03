namespace EShopper.Api.Core
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }

        public JwtSettings JwtSettings { get; set; }
    }
}
