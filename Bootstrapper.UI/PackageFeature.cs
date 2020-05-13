using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using System.Xml.Linq;

namespace Bootstrapper.UI
{
    internal class PackageFeature
    {
        private XElement xElement;

        public PackageFeature(XElement xElement)
        {
            const string PackageIdName = "Package";

            this.xElement = xElement;

            PackageId = xElement.Attribute(PackageIdName).Value;
            Feature = xElement.Attribute(nameof(Feature)).Value;
            Size = xElement.Attribute(nameof(Size)).ToLong();
            Parent = xElement.Attribute(nameof(Parent)).Value;
            Title = xElement.Attribute(nameof(Title)).Value;
            Description = xElement.Attribute(nameof(Description)).Value;
            Display = xElement.Attribute(nameof(Display)).ToInt();
            Level = xElement.Attribute(nameof(Level)).ToInt();
            Directory = xElement.Attribute(nameof(Directory)).Value;
            Attributes = xElement.Attribute(nameof(Attributes)).ToInt();
        }

        public int Attributes { get; private set; }

        public FeatureState CurrentState { get; internal set; } = FeatureState.Unknown;

        public string Description { get; private set; }

        public string Directory { get; private set; }

        public int Display { get; private set; }

        public string Feature { get; private set; }

        public int Level { get; private set; }

        public BundlePackage Package { get; internal set; }

        public string PackageId { get; private set; }

        public string Parent { get; private set; }

        public long Size { get; private set; }

        public string Title { get; private set; }
        public FeatureState PlanState { get; internal set; }

        public override string ToString() => $"{PackageId} - {Title}";
    }
}