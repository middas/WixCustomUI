using Bootstrapper.UI.ViewModels;
using Bootstrapper.UI.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace Bootstrapper.UI
{
    public class BootstrapperEntry : BootstrapperApplication
    {
        private Dispatcher _BootstrapDispatcher;
        private InstallerWindowViewModel _InstallerWindowViewModel;

        protected override void Run()
        {
            WaitForDebugger();

            _BootstrapDispatcher = Dispatcher.CurrentDispatcher;

            // should UI be displayed
            if (Command.Display == Display.Full || Command.Display == Display.Unknown)
            {
                Engine.Log(LogLevel.Verbose, "Launching custom UX");

                _InstallerWindowViewModel = new InstallerWindowViewModel(this);

                InstallerWindow installerWindow = new InstallerWindow
                {
                    DataContext = _InstallerWindowViewModel
                };
                installerWindow.Closed += (s, e) => _BootstrapDispatcher.InvokeShutdown();
                installerWindow.Show();

                Dispatcher.Run();

                Engine.Quit(0);
            }
        }

        private void WaitForDebugger()
        {
            if (Command.GetCommandLineArgs().Contains("DEBUG"))
            {
                Engine.Log(LogLevel.Verbose, "Waiting for debugger to be attached...");

                while (!Debugger.IsAttached)
                {
                    Thread.Sleep(500);
                }

                Debugger.Break();
            }
        }
    }
}