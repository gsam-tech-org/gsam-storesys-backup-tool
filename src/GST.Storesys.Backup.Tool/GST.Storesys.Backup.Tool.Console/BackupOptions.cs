using CommandLine;

namespace GST.Storesys.Backup.Tool.Console
{
    [Verb("backup", HelpText = "Allow operations with backups")]
    public class BackupOptions
    {
        [Option('f', "full", Required = false, HelpText = "Run full backup.")]
        public bool Full { get; set; }

        [Option('e', "empresa", Required = false, HelpText = "Nombre de la empresa.")]
        public string Empresa { get; set; } = "EMPRESA";

        [Option('s', "sucursal", Required = false, HelpText = "Nombre de la sucursal.")]
        public string Sucursal { get; set; } = "S1";
    }
}
