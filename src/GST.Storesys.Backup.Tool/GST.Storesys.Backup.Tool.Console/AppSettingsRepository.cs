using SharpConfig;

namespace GST.Storesys.Backup.Tool.Console
{
    public class AppSettingsRepository
    {
        private const string file = "settings.cfg";

        public AppSettingsRepository()
        {
            if (!System.IO.File.Exists(file))
            {
                System.IO.File.WriteAllText(file, "");
            }
        }
        public AppSettings Load()
        {
            var config = Configuration.LoadFromFile(file);
            var general = config["General"].ToObject<GeneralSettings>();
            var backup = config["Backup"].ToObject<BackupSettings>();
            var settings = new AppSettings
            {
                General = general,
                Backup = backup
            };
            return settings;
        }

        public void Save(AppSettings settings)
        {
            var config = Configuration.LoadFromFile(file);
            var general = SharpConfig.Section.FromObject("General", settings.General);
            config.Add(general);

            var backup = SharpConfig.Section.FromObject("Backup", settings.Backup);
            config.Add(backup);

            config.SaveToFile(file);
        }
    }
}
