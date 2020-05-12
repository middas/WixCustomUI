using Bootstrapper.UI.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Windows.Threading;

namespace Bootstrapper.UI
{
    public class BootstrapperEntry : BootstrapperApplication
    {
        private Dispatcher _BootstrapDispatcher;

        protected override void Run()
        {
            _BootstrapDispatcher = Dispatcher.CurrentDispatcher;

            // should UI be displayed
            if (Command.Display == Display.Full || Command.Display == Display.Unknown)
            {
                Engine.Log(LogLevel.Verbose, "Launching custom UX");

                InstallerWindow installerWindow = new InstallerWindow();
                installerWindow.Closed += (s, e) => _BootstrapDispatcher.InvokeShutdown();
                installerWindow.Show();

                Dispatcher.Run();

                Engine.Quit(0);
            }
        }
    }
}