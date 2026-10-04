using System.Configuration;
using System.Data;
using System.Windows;
using WpfApp1.Data;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (!DbSettings.Cargar())
            {
                MessageBox.Show(@"Falta el archivo C:\ProgramData\Sevicell\config.json",
                                "Sevicell", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }

}
