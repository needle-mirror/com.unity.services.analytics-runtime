using NUnit.Framework;

namespace Unity.Services.Analytics.Internal.Tests
{
    [TestFixture]
    public class EventTests
    {
        [Test]
        public void Simple()
        {
            Event evt = new Event("Foo", 123);
            Assert.IsTrue(evt.Name == "Foo");
            Assert.IsTrue(evt.Version == 123);
            
            evt = new Event("Boo", null);
            Assert.IsTrue(evt.Name == "Boo");
            Assert.IsTrue(evt.Version == null);

        }

        [Test]
        public void ParamsSimple()
        {
            Event evt = new Event("Baz", 1);
            evt.Parameters.Set("ValueFloat", 123.0F);
            evt.Parameters.Set("ValueString", "Bar");

            Assert.IsTrue(evt.Parameters.Data.Count == 2);
            Assert.IsTrue((float)evt.Parameters.Data["ValueFloat"] == 123.0F);
            Assert.IsTrue((string)evt.Parameters.Data["ValueString"] == "Bar");
        }
    }
}
