using Renci.SshNet;
using System;
using System.IO;
using System.IO.Compression;

namespace GST.Storesys.Backup.Tool.Console
{
    public class BackupRunner
    {
        readonly AppSettings settings;
        public BackupRunner(AppSettings settings)
        {
            this.settings = settings;
        }

        public void Start()
        {
            System.Console.WriteLine("Running full backup.");
            string empresa = this.settings.General.Empresa.ToUpperInvariant();
            string sucursal = this.settings.General.Sucursal.ToUpperInvariant();
            string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            string tmp = Path.GetTempPath();


            try
            {
                string sourcePath1 = @"C:\\storesys";
                string backupPath1 = Path.Combine(tmp, $"ST_{empresa}_{sucursal}_{timestamp}");
                string zipPath1 = Path.Combine(tmp, $"ST_{empresa}_{sucursal}_{timestamp}.zip");

                if (Directory.Exists(sourcePath1))
                {
                    CopyDirectory(sourcePath1, backupPath1);
                    System.Console.WriteLine("Backup de storesys realizado con éxito en: " + backupPath1);

                    ZipBackup(backupPath1, zipPath1);
                    System.Console.WriteLine("Backup de storesys comprimido en: " + zipPath1);

                    Directory.Delete(backupPath1, true);
                    System.Console.WriteLine("Directorio de backup eliminado: " + backupPath1);
                }
                else
                {
                    System.Console.WriteLine("El directorio de origen no existe: " + sourcePath1);
                }

                // Subir los backups al servidor
                Store(zipPath1);

                //Eliminar los archivos .Zip
                File.Delete(zipPath1);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error al realizar el backup: " + ex.Message);
            }

            //// Backup de bases
            try
            {
                string sourcePath2 = @"C:\\storesys\\bases";
                string backupPath2 = Path.Combine(tmp, $@"ST-DB_{empresa}_{sucursal}_{timestamp}");
                string zipPath2 = Path.Combine(tmp, $@"ST-DB_{empresa}_{sucursal}_{timestamp}.zip");

                if (Directory.Exists(sourcePath2))
                {
                    CopyDirectory(sourcePath2, backupPath2);
                    System.Console.WriteLine("Backup de bases realizado con éxito en: " + backupPath2);

                    ZipBackup(backupPath2, zipPath2);
                    System.Console.WriteLine("Backup de bases comprimido en: " + zipPath2);

                    Directory.Delete(backupPath2, true);
                    System.Console.WriteLine("Directorio de backup eliminado: " + backupPath2);
                }
                else
                {
                    System.Console.WriteLine("El directorio de origen no existe: " + sourcePath2);
                }

                // Subir los backups al servidor
                Store(zipPath2);

                //Eliminar los archivos .Zip
                File.Delete(zipPath2);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error al realizar el backup: " + ex.Message);
            }


            //// Backup de ocx
            try
            {
                string sourcePath3 = @"C:\\storesys\\ocx";
                string backupPath3 = Path.Combine(tmp, $@"ST-OCX_{empresa}_{sucursal}_{timestamp}");
                string zipPath3 = Path.Combine(tmp, $@"ST-OCX_{empresa}_{sucursal}_{timestamp}.zip");

                if (Directory.Exists(sourcePath3))
                {
                    CopyDirectory(sourcePath3, backupPath3);
                    System.Console.WriteLine("Backup de storesys realizado con éxito en: " + backupPath3);

                    ZipBackup(backupPath3, zipPath3);
                    System.Console.WriteLine("Backup de storesys comprimido en: " + zipPath3);

                    Directory.Delete(backupPath3, true);
                    System.Console.WriteLine("Directorio de backup eliminado: " + backupPath3);
                }
                else
                {
                    System.Console.WriteLine("El directorio de origen no existe: " + sourcePath3);
                }

                // Subir los backups al servidor
                Store(zipPath3);

                //Eliminar los archivos .Zip
                File.Delete(zipPath3);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error al realizar el backup: " + ex.Message);
            }
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            foreach (FileInfo file in dir.GetFiles())
            {
                string tempPath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (DirectoryInfo subdir in dir.GetDirectories())
            {
                if (subdir.Name.Equals("ocx", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string tempPath = Path.Combine(destinationDir, subdir.Name);
                CopyDirectory(subdir.FullName, tempPath);
            }
        }

        private void ZipBackup(string sourceDir, string zipPath)
        {
            try
            {
                if (File.Exists(zipPath))
                {
                    File.SetAttributes(zipPath, FileAttributes.Normal);
                    File.Delete(zipPath);
                    System.Threading.Thread.Sleep(500);
                }

                ZipFile.CreateFromDirectory(sourceDir, zipPath);
                System.Console.WriteLine($"Backup comprimido en: {zipPath}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Error al comprimir {sourceDir}: {ex.Message}");
            }
        }


        private void Store(string source)
        {
            var fi = new FileInfo(source);
            using (var client = new SftpClient(
                settings.Backup.Server,
                settings.Backup.Port,
                settings.Backup.Username,
                settings.Backup.Password))
            {
                System.Console.WriteLine($"Connecting to {settings.Backup.Server}");
                client.Connect();
                var wc = client.WorkingDirectory;
                using (FileStream fs = File.OpenRead(source))
                {
                    var target = "backups/";
                    if (fi.Name.StartsWith("ST-DB"))
                    {
                        target = "bases/";
                    }
                    else if (fi.Name.StartsWith("ST-OCX"))
                    {
                        target = "ocxs/";
                    }

                    client.UploadFile(fs, target + fi.Name, size =>
                    {
                        System.Console.Write(".");
                    });
                    System.Console.WriteLine();
                    System.Console.WriteLine($"{target}{fi.Name} file uploaded.");
                }
                client.Disconnect();
            }
        }
    }
}
