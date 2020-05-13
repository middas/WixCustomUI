using System.Xml.Linq;

namespace Bootstrapper.UI
{
    public static class XAttributeExtensions
    {
        public static int ToInt(this XAttribute xAttribute)
        {
            if (int.TryParse(xAttribute.Value, out int i))
            {
                return i;
            }

            return 0;
        }

        public static long ToLong(this XAttribute xAttribute)
        {
            if (long.TryParse(xAttribute.Value, out long l))
            {
                return l;
            }

            return 0;
        }

        public static bool YesNoToBool(this XAttribute xAttribute)
        {
            const string trueValue = "YES";

            return xAttribute.Value.ToUpper() == trueValue;
        }
    }
}