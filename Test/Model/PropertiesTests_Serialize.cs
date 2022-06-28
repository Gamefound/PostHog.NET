using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PostHog.Model;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Test.Model
{
    [TestClass]
    public class PropertiesTests_Serialize
    {
        [TestMethod]
        public void PropertiesTests_Serialize_ShouldBeSerializedProperly()
        {
            var properties = new Properties()
                .SetEventProperty("event", "event_value")
                .SetUserProperty("user", "userValue")
                .SetUserPopertyOnce("user_once", "user_once_value");

            var json = JsonConvert.SerializeObject(properties);
            var deserializedJson = JsonConvert.DeserializeObject<Properties>(json);

            deserializedJson.Should().NotBeNull();
            deserializedJson.Count.Should().Be(3);

            deserializedJson["event"].Should().Be("event_value");
            deserializedJson["$set"].Should().BeEquivalentTo(new Dictionary<string, object>() { { "user", (JValue)"userValue" } });
            deserializedJson["$set_once"].Should().BeEquivalentTo(new Dictionary<string, object>() { { "user_once", (JValue)"user_once_value" } });
        }
    }
}