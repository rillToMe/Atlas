using System.Windows;
using Atlas.Services;

namespace Atlas
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var configService = new ConfigService();
            var config = configService.LoadConfig();

            Window startWindow;

            if (config.IsFirstRun)
            {
                startWindow = new Views.WelcomeWindow();
            }
            else
            {
                startWindow = new Views.PinEntryWindow();
            }

            startWindow.Show();
        }
    }
}