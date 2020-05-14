using System;
using System.ComponentModel;

namespace Bootstrapper.UI.ViewModels
{
    public class InstallerWindowViewModel : INotifyPropertyChanged
    {
        private readonly BootstrapperEntry bootstrapper;
        private readonly FeaturesControlViewModel featuresControlViewModel;
        private readonly InstallControlViewModel installControlViewModel;

        private ViewPage _CurrentPage;
        private ViewPage[] _Pages;
        private INotifyPropertyChanged _SelectedViewModel;

        // This constructor is used for the design view only
        public InstallerWindowViewModel()
        {
            installControlViewModel = new InstallControlViewModel();
            featuresControlViewModel = new FeaturesControlViewModel();
            Pages = (ViewPage[])Enum.GetValues(typeof(ViewPage));
            OnCurrentPageChanged(); // notify the UI what page we're starting on
        }

        public InstallerWindowViewModel(BootstrapperEntry bootstrapper)
        {
            this.bootstrapper = bootstrapper;
            installControlViewModel = new InstallControlViewModel(bootstrapper);
            featuresControlViewModel = new FeaturesControlViewModel(bootstrapper);
            Pages = (ViewPage[])Enum.GetValues(typeof(ViewPage));
            OnCurrentPageChanged(); // notify the UI what page we're starting on

            bootstrapper.Detect();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum ViewPage
        {
            Installation,
            Features
        }

        public ViewPage CurrentPage
        {
            get => _CurrentPage;
            set
            {
                _CurrentPage = value;
                FirePropertyChanged(nameof(CurrentPage));
                OnCurrentPageChanged();
            }
        }

        public ViewPage[] Pages
        {
            get => _Pages;
            set
            {
                _Pages = value;
                FirePropertyChanged(nameof(Pages));
            }
        }

        public INotifyPropertyChanged SelectedViewModel
        {
            get => _SelectedViewModel;
            set
            {
                _SelectedViewModel = value;
                FirePropertyChanged(nameof(SelectedViewModel));
            }
        }

        private void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void OnCurrentPageChanged()
        {
            switch (CurrentPage)
            {
                case ViewPage.Features:
                    SelectedViewModel = featuresControlViewModel;
                    break;

                case ViewPage.Installation:
                    SelectedViewModel = installControlViewModel;
                    break;
            }
        }
    }
}