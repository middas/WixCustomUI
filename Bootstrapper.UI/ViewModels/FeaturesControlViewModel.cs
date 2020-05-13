using System;
using System.ComponentModel;

namespace Bootstrapper.UI.ViewModels
{
    public class FeaturesControlViewModel : INotifyPropertyChanged
    {
        private readonly BootstrapperEntry bootstrapper;

        private bool _InstallFeature1;

        // This constructor is used for the design view only
        public FeaturesControlViewModel()
        {
            InstallFeature1 = true;
        }

        public FeaturesControlViewModel(BootstrapperEntry bootstrapper)
        {
            this.bootstrapper = bootstrapper;

            SetUiFromInstallState();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool InstallFeature1
        {
            get => _InstallFeature1;
            set
            {
                _InstallFeature1 = value;
                FirePropertyChanged(nameof(InstallFeature1));
                OnInstallFeature1Changed();
            }
        }

        private void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void OnInstallFeature1Changed()
        {
            //TODO
        }

        private void SetUiFromInstallState()
        {
            //TODO
        }
    }
}