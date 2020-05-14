using Bootstrapper.UI.MVVM.Commands;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Bootstrapper.UI.ViewModels
{
    public class InstallControlViewModel : INotifyPropertyChanged
    {
        private readonly BootstrapperEntry bootstrapper;

        private bool _IsError;
        private bool _IsInstalling;
        private bool _IsUninstall = false;
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
            this.bootstrapper = bootstrapper;

            bootstrapper.DetectComplete += (sender, args) => SetUiFromInstallState();
            bootstrapper.PlanComplete += (sender, args) => PlanComplete();
            bootstrapper.PlanMsiFeature += (sender, args) => PlanFeature(args);
            bootstrapper.PlanTargetMsiPackage += (sender, args) => PlanPackage(args);
            bootstrapper.ApplyBegin += (sender, args) => IsInstalling = true;
            bootstrapper.ApplyComplete += (sender, args) => ApplyComplete(args);
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

        private void ApplyComplete(ApplyCompleteEventArgs args)
        {
            IsInstalling = false;
            string installMessage = _IsUninstall ? "uninstall" : "install";
            _IsUninstall = false;

            if (args.Status == 0)
            {
                ResultMessage = $"Successfully {installMessage}ed";
                IsError = false;
            }
            else
            {
                ResultMessage = $"Failed to {installMessage}";
                IsError = true;
            }

            bootstrapper.Detect();
        }

        private void FirePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void Install()
        {
            try
            {
                ValidateBootstrapper();

                var package = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName);
                package.PlanState = RequestState.Present;
                var primaryFeature = package.Features.First(f => f.Feature == BootstrapperEntry.PrimaryFeatureName);
                primaryFeature.PlanState = FeatureState.Local;

                bootstrapper.Engine.Log(LogLevel.Standard, $"Package: {package.DisplayName}, Plan: {package.PlanState}");
                bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {primaryFeature.Feature}, Plan: {primaryFeature.PlanState}");

                bootstrapper.Plan(LaunchAction.Install);
            }
            catch (Exception ex)
            {
                ResultMessage = $"Install failed:\n{ex.Message}";
                IsError = true;
            }
        }

        private void PlanComplete()
        {
            bootstrapper.Execute();
        }

        private void PlanFeature(PlanMsiFeatureEventArgs args)
        {
            var feature = bootstrapper.Packages.First(pkg => pkg.Id == args.PackageId).Features.First(f => f.Feature == args.FeatureId);
            args.State = feature.PlanState;

            bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {feature.Feature}, Plan: {feature.PlanState}");
        }

        private void PlanPackage(PlanTargetMsiPackageEventArgs args)
        {
            var package = bootstrapper.Packages.First(pkg => pkg.Id == args.PackageId);
            args.State = package.PlanState;

            bootstrapper.Engine.Log(LogLevel.Standard, $"Package: {package.DisplayName}, Plan: {package.PlanState}");
        }

        private void RepairModify()
        {
            try
            {
                ValidateBootstrapper();

                bool isRepair = bootstrapper.Packages.SelectMany(pkg => pkg.Features).All(f => f.PlanState == f.CurrentState);

                var package = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName);
                package.PlanState = isRepair ? RequestState.Repair : RequestState.Present;
                var primaryFeature = package.Features.First(f => f.Feature == BootstrapperEntry.PrimaryFeatureName);
                primaryFeature.PlanState = FeatureState.Local;

                bootstrapper.Engine.Log(LogLevel.Standard, $"Package: {package.DisplayName}, Plan: {package.PlanState}");
                bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {primaryFeature.Feature}, Plan: {primaryFeature.PlanState}");

                bootstrapper.Plan(isRepair ? LaunchAction.Repair : LaunchAction.Modify);
            }
            catch (Exception ex)
            {
                ResultMessage = $"Repair/modify failed:\n{ex.Message}";
                IsError = true;
            }
        }

        private void SetUiFromInstallState()
        {
            ShowInstall = !bootstrapper.IsInstalled;
            ShowUpgrade = bootstrapper.IsUpgrade;
            ShowRepairUninstall = !ShowInstall && !ShowUpgrade;
        }

        private void Uninstall()
        {
            try
            {
                ValidateBootstrapper();

                var package = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName);
                package.PlanState = RequestState.ForceAbsent;

                bootstrapper.Engine.Log(LogLevel.Standard, $"Package: {package.DisplayName}, Plan: {package.PlanState}");

                foreach (var feature in package.Features)
                {
                    feature.PlanState = FeatureState.Absent;

                    bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {feature.Feature}, Plan: {feature.PlanState}");
                }

                _IsUninstall = true;
                bootstrapper.Plan(LaunchAction.Uninstall);
            }
            catch (Exception ex)
            {
                ResultMessage = $"Uninstall failed:\n{ex.Message}";
                IsError = true;
            }
        }

        private void Upgrade()
        {
            try
            {
                ValidateBootstrapper();

                var package = bootstrapper.Packages.First(pkg => pkg.Id == BootstrapperEntry.PrimaryPackageName);
                package.PlanState = RequestState.Present;
                var primaryFeature = package.Features.First(f => f.Feature == BootstrapperEntry.PrimaryFeatureName);
                primaryFeature.PlanState = FeatureState.Local;

                bootstrapper.Engine.Log(LogLevel.Standard, $"Package: {package.DisplayName}, Plan: {package.PlanState}");
                bootstrapper.Engine.Log(LogLevel.Standard, $"Feature: {primaryFeature.Feature}, Plan: {primaryFeature.PlanState}");

                bootstrapper.Plan(LaunchAction.Install);
            }
            catch (Exception ex)
            {
                ResultMessage = $"Upgrade failed:\n{ex.Message}";
                IsError = true;
            }
        }

        private void ValidateBootstrapper()
        {
            if (bootstrapper == null)
            {
                throw new InvalidOperationException("The bootstrapper was not initialized properly.");
            }
        }
    }
}