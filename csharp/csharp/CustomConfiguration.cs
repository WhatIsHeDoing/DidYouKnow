using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Specialized;
using System.Configuration;

namespace csharp
{
    /// <summary>
    /// Tests custom application configuration.
    /// </summary>
    [TestClass]
    public class CustomConfiguration
    {
        /// <summary>
        /// Accesses a custom key within the app.config.
        /// </summary>
        [TestMethod]
        public void AccessCustomKey()
        {
            var customKeyValue = ConfigurationManager.AppSettings["CustomKey"];
            Assert.AreEqual("test", customKeyValue);
        }

        /// <summary>
        /// Accesses an entire custom section via a path.
        /// The structure of the section is defined
        /// in the configSections of the app.config.
        /// </summary>
        [TestMethod]
        public void AccessCustomSection()
        {
            ConfigurationManager.OpenExeConfiguration
                (ConfigurationUserLevel.None);

            var customSection = ConfigurationManager
                .GetSection("CustomSection/Group/Data");

            Assert.IsNotNull(customSection);

            var collection = customSection as NameValueCollection;
            Assert.IsNotNull(collection);

            var customKeyValue = collection["CustomKey"];
            Assert.AreEqual("testing", customKeyValue);
        }
    }
}
