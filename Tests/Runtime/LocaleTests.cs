using NUnit.Framework;

namespace Unity.Services.Analytics.Internal.Tests
{
    public class LocaleTests
    {
        [Test]
        public void LocaleTestsSystem()
        {
            Assert.AreEqual("", Locale.SystemCulture().ToString());
        }
    }
}
