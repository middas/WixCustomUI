using Bootstrapper.UI.MVVM.Commands;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Bootstrapper.UI.ViewModels
{
    public class InstallControlViewModel : INotifyPropertyChanged
    {
        private readonly BootstrapperEntry Bootstrapper;

        private bool _IsError;
        private bool _IsInstalling;
        private string _ResultMessage;
        private bool _ShowInstall;
        private bool _ShowRepairUninstall;
        private bool _ShowUpgrade;

        // This constructor is used for the design view only
        public InstallControlViewModel()
        {
            ShowInstall = true;
            ResultMessage = "The constructor for UI only was used.";
            IsError = true;
        }

        public InstallControlViewModel(BootstrapperEntry bootstrapper)
        {
            Bootstrapper = bootstrapper;

            SetUiFromInstallState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand InstallCommand { get => new ActionCommand(Install); }

        public bool IsError
        {
            get => _IsError;
            set
            {
                _IsError = value;
                FirePropertyChanged(nameof(IsError));
            }
        }

        public bool IsInstalling
        {
            get => _IsInstalling;
            set
            {
                _IsInstalling = value;
                FirePropertyChanged(nameof(IsInstalling));
            }
        }

        public ICommand RepairModifyCommand { get => new ActionCommand(RepairModify); }

        public string ResultMessage
        {
            get => _ResultMessage;
            set
            {
                _ResultMessage = value;
                FirePropertyChanged(nameof(ResultMessage));
            }
        }

        public bool ShowInstall
        {
            get => _ShowInstall;
            set
            {
                _ShowInstall = value;
                FirePropertyChanged(nameof(ShowInstall));
            }
        }

        public bool ShowRepairUninstall
        {
            get => _ShowRepairUninstall;
            set
            {
                _ShowRepairUninstall = value;
                FirePropertyChanged(nameof(ShowRepairUninstall));
            }
        }

        public bool ShowUpgrade
        {
            get => _ShowUpgrade;
            set
            {
                _ShowUpgrade = value;
                FirePropertyChanged(nameof(ShowUpgrade));
            }
        }

        public ICommand UninstallCommand { get => new ActionCommand(Uninstall); }

        public ICommand UpgradeCommand { get => new ActionCommand(Upgrade); }

        private void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Install()
        {
            throw new NotImplementedException();
        }

        private void RepairModify()
        {
            throw new NotImplementedException();
        }

        private void SetUiFromInstallState()
        {
            throw new NotImplementedException();
        }

        private void Uninstall()
        {
            throw new NotImplementedException();
        }

        private void Upgrade()
        {
            throw new NotImplementedException();
        }
    }
}