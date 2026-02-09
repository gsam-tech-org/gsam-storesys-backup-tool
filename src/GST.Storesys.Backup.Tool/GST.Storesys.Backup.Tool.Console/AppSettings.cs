namespace GST.Storesys.Backup.Tool.Console
{
    public class AppSettings
    {
        public GeneralSettings General { get; set; } = new GeneralSettings();

        public BackupSettings Backup { get; set; } = new BackupSettings();

    }
}
