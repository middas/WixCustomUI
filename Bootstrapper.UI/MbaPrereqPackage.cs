using System.Xml.Linq;

namespace Bootstrapper.UI
{
    internal class MbaPrereqPackage
    {
        private XElement xElement;

        public MbaPrereqPackage(XElement xElement)
        {
            this.xElement = xElement;

            PackageId = xElement.Attribute(nameof(PackageId)).Value;
            LicenseUrl = xElement.Attribute(nameof(LicenseUrl)).Value;
        }

        public string LicenseUrl { get; private set; }

        public string PackageId { get; private set; }

        public override string ToString() => PackageId;
    }
}