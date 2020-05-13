using Bootstrapper.UI.ViewModels;
using Bootstrapper.UI.Views;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Bootstrapper.UI
{
    public class BootstrapperEntry : BootstrapperApplication
    {
        internal const string PrimaryFeatureName = "ProductFeature";
        internal const string PrimaryPackageName = "Msi_Installer";

        private readonly XNamespace ManifestName = "http://schemas.microsoft.com/wix/2010/BootstrapperApplicationData";

        private Dispatcher _BootstrapDispatcher;
        private InstallerWindowViewModel _InstallerWindowViewModel;

        public bool IsInstalled
        {
            get
            {
                var package = Packages.First(pkg => pkg.Id == PrimaryPackageName);

                return package.CurrentState == PackageState.Present;
            }
        }

        public bool IsUpgrade
        {
            get
            {
                var package = Packages.First(pkg => pkg.Id == PrimaryPackageName);

                return package.RelatedOperation == RelatedOperation.MajorUpgrade || package.RelatedOperation == RelatedOperation.MinorUpdate;
            }
        }

        internal BundlePackage[] Packages { get; private set; }

        internal void Detect()
        {
            Engine.Detect();
        }

        internal void Execute()
        {
            Engine.Apply(Process.GetCurrentProcess().MainWindowHandle);
        }

        internal void Plan(LaunchAction launchAction)
        {
            Engine.Plan(launchAction);
        }

        protected override void OnDetectMsiFeature(DetectMsiFeatureEventArgs args)
        {
            base.OnDetectMsiFeature(args);

            var package = Packages.FirstOrDefault(pkg => pkg.Id == args.PackageId);
            if (package != null)
            {
                var feature = package.Features.FirstOrDefault(f => f.Feature == args.FeatureId);
                if (feature != null)
                {
                    feature.CurrentState = args.State;
                }
            }
        }

        protected override void OnDetectPackageComplete(DetectPackageCompleteEventArgs args)
        {
            base.OnDetectPackageComplete(args);

            var package = Packages.FirstOrDefault(pkg => pkg.Id == args.PackageId);
            if (package != null)
            {
                package.CurrentState = args.State;
            }
        }

        protected override void OnDetectRelatedMsiPackage(DetectRelatedMsiPackageEventArgs args)
        {
            base.OnDetectRelatedMsiPackage(args);

            var package = Packages.First(pkg => pkg.Id == args.PackageId);
            package.InstalledVersion = args.Version;
            package.RelatedOperation = args.Operation;
        }

        protected override void Run()
        {
            WaitForDebugger();

            InitializePackages();

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
            else
            {
                DetectComplete += (sender, args) => Plan(Command.Action);
                PlanComplete += (sender, args) => Execute();
                ExecuteComplete += (sender, args) =>
                {
                    Engine.Quit(args.Status);
                    _BootstrapDispatcher.InvokeShutdown();
                };

                Detect();

                Dispatcher.Run();
            }
        }

        private void InitializePackages()
        {
            const string DataFilePathName = "BootstrapperApplicationData.xml";
            const string ApplicationDataNamespace = "BootstrapperApplicationData";
            const string MbaPrereqNamespace = "WixMbaPrereqInformation";
            const string PackageNamespace = "WixPackageProperties";
            const string FeatureNamespace = "WixPackageFeatureInfo";

            var workingDir = Path.GetDirectoryName(GetType().Assembly.Location);
            var dataFilePath = Path.Combine(workingDir, DataFilePathName);
            XElement applicationData = null;

            try
            {
                using (var reader = new StreamReader(dataFilePath))
                {
                    var xml = reader.ReadToEnd();
                    var xDoc = XDocument.Parse(xml);
                    applicationData = xDoc.Element(ManifestName + ApplicationDataNamespace);
                }
            }
            catch (Exception ex)
            {
                Engine.Log(LogLevel.Error, $"Unable to parse {DataFilePathName}.\nReason: {ex.Message}");
            }

            var mbaPrereqs = applicationData.Descendants(ManifestName + MbaPrereqNamespace)
                                            .Select(x => new MbaPrereqPackage(x));
            // exclude prereq packages
            Packages = applicationData.Descendants(ManifestName + PackageNamespace)
                                          .Select(x => new BundlePackage(x))
                                          .Where(pkg => !mbaPrereqs.Any(preReq => preReq.PackageId == pkg.Id))
                                          .ToArray();

            // get features and associate with their package
            var featureNodes = applicationData.Descendants(ManifestName + FeatureNamespace);
            foreach (var featureNode in featureNodes)
            {
                var feature = new PackageFeature(featureNode);
                var parentPkg = Packages.First(pkg => pkg.Id == feature.PackageId);
                parentPkg.Features.Add(feature);
                feature.Package = parentPkg;
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