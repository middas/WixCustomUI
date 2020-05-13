using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Bootstrapper.UI
{
    internal class BundlePackage
    {
        private XElement xElement;

        public BundlePackage(XElement xElement)
        {
            const string IdName = "Package";

            this.xElement = xElement;

            Id = xElement.Attribute(IdName).Value;
            Vital = xElement.Attribute(nameof(Vital)).YesNoToBool();
            DisplayName = xElement.Attribute(nameof(DisplayName)).Value;
            DownloadSize = xElement.Attribute(nameof(DownloadSize)).ToLong();
            PackageSize = xElement.Attribute(nameof(PackageSize)).ToLong();
            InstalledSize = xElement.Attribute(nameof(InstalledSize)).ToLong();
            PackageType = xElement.Attribute(nameof(PackageType)).Value;
            Permanent = xElement.Attribute(nameof(Permanent)).YesNoToBool();
            LogPathVariable = xElement.Attribute(nameof(LogPathVariable)).Value;
        }

        public PackageState CurrentState { get; internal set; } = PackageState.Unknown;

        public string DisplayName { get; private set; }

        public long DownloadSize { get; private set; }

        public List<PackageFeature> Features { get; private set; } = new List<PackageFeature>();

        public string Id { get; private set; }

        public long InstalledSize { get; private set; }

        public Version InstalledVersion { get; internal set; }

        public string LogPathVariable { get; private set; }

        public long PackageSize { get; private set; }

        public string PackageType { get; private set; }

        public bool Permanent { get; private set; }

        public RelatedOperation RelatedOperation { get; internal set; } = RelatedOperation.None;

        public bool Vital { get; private set; }

        public override string ToString() => DisplayName;
    }
}