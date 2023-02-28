namespace Flight.WEBAPP.Common.Models
{
    public class ConfigurationModel
    {

        private IConfiguration Settings;
        private AppConfiguration _AppConfiguration;
        public AppConfiguration AppConfiguration
        {
            get
            {
                return _AppConfiguration;
            }
        }
        public ConfigurationModel()
        {
            _AppConfiguration = new AppConfiguration();
            Settings = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            Settings.GetSection("AppConfiguration").Bind(_AppConfiguration);
        }
    }
}
