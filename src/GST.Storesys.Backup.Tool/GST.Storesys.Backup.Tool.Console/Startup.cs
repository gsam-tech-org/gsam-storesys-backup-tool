using CommandLine;

namespace GST.Storesys.Backup.Tool.Console
{
    internal class Startup
    {
        static void Main(string[] args)
        {
            // https://github.com/commandlineparser/commandline
            Parser.Default.ParseArguments<BackupOptions>(args)
            .WithParsed<BackupOptions>(bo =>
            {
                if (bo.Full)
                {
                    var settings = new AppSettingsRepository().Load();
                    if (!string.IsNullOrEmpty(bo.Empresa)) settings.General.Empresa = bo.Empresa;
                    if (!string.IsNullOrEmpty(bo.Sucursal)) settings.General.Sucursal = bo.Sucursal;

                    settings.Backup.Server = "sftp.server.hqs.com.ar";
                    settings.Backup.Port = 5808;
                    settings.Backup.Username = "leandro";
                    settings.Backup.Password = "Gsam2026";

                    var runner = new BackupRunner(settings);
                    runner.Start();
                }
            });
        }
    }
}
