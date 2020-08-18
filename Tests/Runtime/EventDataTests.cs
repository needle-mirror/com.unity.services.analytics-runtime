using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Unity.Services.Analytics.Internal.Tests
{
    [TestFixture]
    public class EventDataTests
    {
        EventData m_Data;

        [SetUp]
        public void SetUp()
        {
            m_Data = new EventData();
        }

        [Test]
        public void NewEventDataHasEmptyDictionary()
        {
            Assert.IsNotNull(m_Data.Data);
            Assert.AreEqual(0, m_Data.Data.Count);
        }

        [Test]
        public void Set_SetsFloatValue()
        {
            m_Data.Set("float", 13.37f);

            Assert.IsTrue(m_Data.Data["float"] is float);
            Assert.AreEqual(13.37f, m_Data.Data["float"]);
        }
        
        [Test]
        public void Set_SetsDoubleValue()
        {
            m_Data.Set("double", 13.37);
            
            Assert.IsTrue(m_Data.Data["double"] is double);
            Assert.AreEqual(13.37, m_Data.Data["double"]);
        }

        [Test]
        public void Set_SetsBooleanValue()
        {
            m_Data.Set("bool", true);
            
            Assert.IsTrue(m_Data.Data["bool"] is bool);
            Assert.AreEqual(true, m_Data.Data["bool"]);
        }
        
        [Test]
        public void Set_SetsIntValue()
        {
            m_Data.Set("int", 1337);
            
            Assert.IsTrue(m_Data.Data["int"] is int);
            Assert.AreEqual(1337, m_Data.Data["int"]);
        }
        
        [Test]
        public void Set_SetsLongValue()
        {
            m_Data.Set("long", 31337L);
            
            Assert.IsTrue(m_Data.Data["long"] is long);
            Assert.AreEqual(31337L, m_Data.Data["long"]);
        }

        [Test]
        public void Set_SetsStringValue()
        {
            m_Data.Set("string", "hello there");
            
            Assert.IsTrue(m_Data.Data["string"] is string);
            Assert.AreEqual("hello there", m_Data.Data["string"]);
        }

        [Test]
        public void Set_SetsObjectValue()
        {
            object actuallyAString = "hello there";
            
            m_Data.Set("string", actuallyAString);
            
            Assert.IsTrue(m_Data.Data["string"] is string);
            Assert.AreEqual("hello there", m_Data.Data["string"]);
        }
    }
}
