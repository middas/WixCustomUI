using System;
using System.ComponentModel;

namespace Bootstrapper.UI.ViewModels
{
    public class InstallerWindowViewModel : INotifyPropertyChanged
    {
        private readonly InstallControlViewModel installControlViewModel;

        private ViewPage _CurrentPage;
        private ViewPage[] _Pages;
        private INotifyPropertyChanged _SelectedViewModel;

        public InstallerWindowViewModel()
        {
            installControlViewModel = new InstallControlViewModel();
            Pages = (ViewPage[])Enum.GetValues(typeof(ViewPage));
            OnCurrentPageChanged(); // notify the UI what page we're starting on
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
            switch(CurrentPage)
            {
                case ViewPage.Features:
                    break;
                case ViewPage.Installation:
                    SelectedViewModel = installControlViewModel;
                    break;
            }
        }
    }
}