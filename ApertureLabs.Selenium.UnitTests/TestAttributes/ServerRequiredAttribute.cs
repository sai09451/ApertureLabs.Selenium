using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ApertureLabs.Selenium.UnitTests.TestAttributes
{
    public class ServerRequiredAttribute : TestPropertyAttribute
    {
        public const string AttributeName = "ServerRequired";

        public ServerRequiredAttribute() : base(AttributeName, true.ToString())
        { }
    }
}
