using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Unity.Services.Analytics.Internal.Tests
{
    [TestFixture]
    public class BufferTests
    {
        readonly string m_CacheFile = $"{Application.persistentDataPath}/eventcache";
        Analytics.Internal.Buffer m_Buf;
        
        [SetUp]
        public void SetUp()
        {
            m_Buf = new Analytics.Internal.Buffer();
            m_Buf.SessionID = "SomeSessionID";
            m_Buf.UserID = "SomeUserID";
        }
        
        [TearDown]
        public void TearDown()
        {
            m_Buf.ClearDiskCache();
        }
        
        [Test]
        public void SimpleEvent()
        {
            // Push an event then serialize it.
            // We should get some text, and after that the buffer should no longer
            // be serializable.
            
            string serialize = m_Buf.Serialize();
            Assert.IsNull(serialize);
            
            m_Buf.PushStartEvent("EmptyEvent", DateTime.Now, 123);
            m_Buf.PushEndEvent();

            serialize = m_Buf.Serialize();
            Assert.IsTrue(serialize != null);
            
            serialize = m_Buf.Serialize();
            Assert.IsNull(serialize);
        }

        [Serializable]
        class TestEventList
        {
            [Serializable]
            internal class TestEvent
            {
                public string eventName = "_eventName";
                public string userID = "_userID";
                public string sessionID = "_sessionID";
                public string eventTimestamp = "_eventTimeStamp";
                public TestEventParams eventParams = new TestEventParams();
            }

            [Serializable]
            internal class TestEventParams
            {
                public string test_int =  "_test_int";
                public string test_string = "_test_string";
            }
            
        }

        [Test]
        public void SmallEvent()
        {
            m_Buf.PushStartEvent("EmptyEvent", DateTime.Now, 1);
            m_Buf.PushInt(8, "test_int");
            m_Buf.PushString("test", "test_string");
            m_Buf.PushEndEvent();

            string serialize = m_Buf.Serialize();

            Assert.IsNotNull(serialize);
            var recreated = JsonUtility.FromJson<TestEventList>(serialize);
            Assert.IsNotNull(recreated);
        }

        [Test]
        public void EmptyEventList()
        {
            string serialize = m_Buf.Serialize();
            Assert.IsNull(serialize);
        }

        [Test]
        public void EventListWithBooleanValues()
        {
            m_Buf.PushBool(true, "boolParameter");
            m_Buf.PushBool(false);
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"boolParameter\":true,false]}");
        }

        [Test]
        public void EventListWithTimestampValue()
        {
            DateTime date = DateTime.Now;
            string dateString = date.ToString("yyyy-MM-dd HH:mm:ss");
            m_Buf.PushTimestamp(date, "timestampParameter");
            string serialize = m_Buf.Serialize();

            Assert.AreEqual(serialize, $"{{\"eventList\":[\"timestampParameter\":\"{dateString}\"]}}");
        }

        [Test]
        public void EventListWithEmptyObjectParameter()
        {
            m_Buf.PushObjectStart("testParameter");
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"testParameter\":{}]}");
        }

        [Test]
        public void EventListWithObjectParameterContainingIntegerParameter()
        {
            m_Buf.PushObjectStart("testParameter");
            m_Buf.PushInt(123, "intParameter");
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"testParameter\":{\"intParameter\":123}]}");
        }

        [Test]
        public void EventListWithObjectParameterContainingMultipleParameters()
        {
            m_Buf.PushObjectStart("testParameter");
            m_Buf.PushInt(123, "intParameter");
            m_Buf.PushString("stringValue", "stringParameter");
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"testParameter\":{\"intParameter\":123,\"stringParameter\":\"stringValue\"}]}");
        }

        [Test]
        public void EventListWithObjectParameterContainingEmptyNestedObject()
        {
            m_Buf.PushObjectStart("mainParameter");
            m_Buf.PushObjectStart("nestedParameter");
            m_Buf.PushObjectEnd();
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"mainParameter\":{\"nestedParameter\":{}}]}");
        }

        [Test]
        public void EventListWithObjectParameterContainingNestedObjectWithOneParameter()
        {
            m_Buf.PushObjectStart("mainParameter");
            m_Buf.PushObjectStart("nestedParameter");
            m_Buf.PushInt(123, "intParameter");
            m_Buf.PushObjectEnd();
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"mainParameter\":{\"nestedParameter\":{\"intParameter\":123}}]}");
        }

        [Test]
        public void EventListWithObjectParameterContainingNestedObjectWithMultipleParameters()
        {
            m_Buf.PushObjectStart("mainParameter");
            m_Buf.PushObjectStart("nestedParameter");
            m_Buf.PushInt(123, "intParameter");
            m_Buf.PushString("stringValue", "stringParameter");
            m_Buf.PushObjectEnd();
            m_Buf.PushObjectEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"mainParameter\":{\"nestedParameter\":{\"intParameter\":123,\"stringParameter\":\"stringValue\"}}]}");
        }

        [Test]
        public void EventListWithEmptyArrayParameter()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingStringParameter()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushString("stringElement");
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[\"stringElement\"]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingIntegerParameters()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushInt(123);
            m_Buf.PushInt64(123);
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[123,123]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingFloatParameter()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushFloat(123.5F);
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[123.5]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingDoubleParameter()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushDouble(123.23);
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[123.23]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingEmptyObjectParameter()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushObjectStart();
            m_Buf.PushObjectEnd();
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[{}]]}");
        }

        [Test]
        public void EventListWithArrayParameterContainingMultipleObjectParameters()
        {
            m_Buf.PushArrayStart("arrayParameter");
            m_Buf.PushObjectStart();
            m_Buf.PushString("stringElement11", "stringParameter");
            m_Buf.PushInt(1, "intParameter");
            m_Buf.PushObjectEnd();
            m_Buf.PushObjectStart();
            m_Buf.PushString("stringElement22", "stringParameter");
            m_Buf.PushInt(2, "intParameter");
            m_Buf.PushObjectEnd();
            m_Buf.PushArrayEnd();
            string serialize = m_Buf.Serialize();
            Assert.AreEqual(serialize, "{\"eventList\":[\"arrayParameter\":[{\"stringParameter\":\"stringElement11\",\"intParameter\":1},{\"stringParameter\":\"stringElement22\",\"intParameter\":2}]]}");
        }

        [Test]
        public void Constructor_NoCacheFile_DoesNothing()
        {
            string serialized = m_Buf.Serialize();
            Assert.IsNull(serialized);
        }

        [Test]
        public void Constructor_CacheFile_ReadsFromDiskIntoBuffer_DeletesFile()
        {
            PushTestData();
            m_Buf.FlushToDisk();

            Buffer newBuffer = new Buffer();
            newBuffer.UserID = "US3R_1D";
            newBuffer.SessionID = "S3SS10N_1D";
            string serialized = newBuffer.Serialize();
            Assert.IsTrue(serialized.Length > 0);
            Assert.IsFalse(File.Exists(m_CacheFile));
        }

        [Test]
        public void FlushToDisk_Repeatedly_KeepsAppendingFile()
        {
            long currentFileSize = 0;
            for (int attempt = 0; attempt < 5; attempt++)
            {
                m_Buf.PushString($"Attempt number: {attempt}");
                m_Buf.FlushToDisk();

                long newFileSize = new FileInfo(m_CacheFile).Length;
                Assert.IsTrue(newFileSize > currentFileSize);
                currentFileSize = newFileSize;
            }
        }

        [Test]
        public void FlushToDisk_ReadBackIn_SerializesToSameOutput()
        {
            PushTestData();

            string serialized = m_Buf.Serialize();

            PushTestData();

            m_Buf.FlushToDisk();
            m_Buf.LoadFromDisk();

            string flushed = m_Buf.Serialize();

            Assert.AreEqual(serialized, flushed);
        }

        [Test]
        public void FlushToDisk_CacheFileIsTooLarge_DoesNotFlush()
        {
            for (int i = 0; i < 1024 * 5; i++)
            {
                m_Buf.PushString(new string('a', 1024), "big_data_item");
            }

            m_Buf.FlushToDisk();

            long fileSizeAfterFirstFlush = new FileInfo(m_CacheFile).Length;

            m_Buf.PushString("This item won't get flushed because the cache is full.", "data_item");

            m_Buf.FlushToDisk();

            Assert.AreEqual(fileSizeAfterFirstFlush, new FileInfo(m_CacheFile).Length);
            Assert.IsTrue(m_Buf.Serialize().Length > 0);
        }

        private void PushTestData()
        {
            m_Buf.PushString($"Hello there! {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}", "leet_data_item");
            m_Buf.PushBool(true, "boolean_data_item");
            m_Buf.PushDouble(0.451, "double_data_item");
            m_Buf.PushInt(1337, "int_data_item");
            m_Buf.PushInt64(31337, "long_data_item");
            m_Buf.PushTimestamp(DateTime.UtcNow, "datetime_data_item");
            m_Buf.PushFloat(0.451f, "float_data_item");
        }
    }
}