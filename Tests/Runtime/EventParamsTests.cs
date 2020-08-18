using NUnit.Framework;

namespace Unity.Services.Analytics.Internal.Tests
{
    [TestFixture]
    public class EventParamsTests
    {
        [Test]
        public void ValueTypes()
        {
            EventData ed = new EventData();
            
            ed.Set("Float", 123.0F);
            ed.Set("Double", 234.0);
            ed.Set("BoolTrue", true);
            ed.Set("BoolFalse", false);
            ed.Set("Integer", 345);
            ed.Set("Int64", 456L);
            ed.Set("String", "StringData");
            
            Assert.IsTrue((float)ed.Data["Float"] == 123.0F);
            Assert.IsTrue((double)ed.Data["Double"] == 234.0);
            Assert.IsTrue((bool)ed.Data["BoolTrue"] == true);
            Assert.IsTrue((bool)ed.Data["BoolFalse"] == false);
            Assert.IsTrue((int)ed.Data["Integer"] == 345);
            Assert.IsTrue((System.Int64)ed.Data["Int64"] == 456L);
            Assert.IsTrue((string)ed.Data["String"] == "StringData");
        }

        [Test]
        public void AddStdData()
        {
            EventData ed = new EventData();
            
            ed.AddStdParamData("method", "123abc");

            Assert.IsTrue(ed.Data.Count == 9);
            Assert.IsTrue((string)ed.Data["sdkMethod"] == "method");
            Assert.IsTrue((string)ed.Data["uasUserID"] == "123abc");
            Assert.IsTrue(ed.Data.ContainsKey("platform"));
            Assert.IsTrue(ed.Data.ContainsKey("batteryLoad"));
            Assert.IsTrue(ed.Data.ContainsKey("connectionType"));
            Assert.IsTrue(ed.Data.ContainsKey("userCountry"));
            Assert.IsTrue(ed.Data.ContainsKey("buildGUUID"));
            Assert.IsTrue(ed.Data.ContainsKey("gameBundleID"));
        }
    }
}
