using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfApp1.Data
{
    /// <summary>
    /// Lee la cadena de conexion de C:\ProgramData\Sevicell\config.json
    /// (cada PC tiene su archivo; la cadena ya no vive en el codigo).
    /// </summary>
    public static class DbSettings
    {
        private static readonly string Ruta = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Sevicell", "config.json");

        public static string ConnectionString { get; private set; }

        public static bool Cargar()
        {
            if (!File.Exists(Ruta)) return false;

            using var doc = JsonDocument.Parse(File.ReadAllText(Ruta));
            if (doc.RootElement.TryGetProperty("ConnectionString", out var valor))
                ConnectionString = valor.GetString();

            return !string.IsNullOrWhiteSpace(ConnectionString);
        }
    }
}
