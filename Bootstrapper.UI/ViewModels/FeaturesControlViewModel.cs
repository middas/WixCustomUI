using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.ComponentModel;
using System.Linq;

namespace Bootstrapper.UI.ViewModels
{
    public class FeaturesControlViewModel : INotifyPropertyChanged
    {
        private const string FeatureName = "OptionalFeature";

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

            bootstrapper.DetectComplete += (sender, args) => SetUiFromInstallState();
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
            if (bootstrapper != null)
            {
                var feature = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName).Features.First(f => f.Feature == FeatureName);
                feature.PlanState = InstallFeature1 ? FeatureState.Local : FeatureState.Absent;

                bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {feature.Feature}, Plan: {feature.PlanState}");
            }
        }

        private void SetUiFromInstallState()
        {
            if (bootstrapper != null)
            {
                if (!bootstrapper.IsInstalled)
                {
                    InstallFeature1 = true;
                }
                else
                {
                    InstallFeature1 = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName)
                                                  .Features.First(f => f.Feature == FeatureName)
                                                  .CurrentState == FeatureState.Local;
                }
            }
        }
    }
}