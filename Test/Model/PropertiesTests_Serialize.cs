using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostHog.Model;
using System.Text.Json;

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
                .SetUserPropertyOnce("user_once", "user_once_value");

            var json = JsonSerializer.Serialize(properties);

            json.Should().Be("{\"event\":\"event_value\",\"$set\":{\"user\":\"userValue\"},\"$set_once\":{\"user_once\":\"user_once_value\"}}");
        }
    }
}
