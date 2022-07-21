using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostHog.Model;
using System.Collections.Generic;
using System.Linq;

namespace Test.Model
{
    [TestClass]
    public class PropertiesTests_SetUserPropertyOnce_ShouldHaveValidValue
    {
        [TestMethod]
        public void PropertiesTests_SetEventProperty_ShouldAddEventProperty()
        {
            var properties = new Properties().SetUserPopertyOnce("key", new [] { 1, 2 ,3 });

            properties.Should().HaveCount(1);
            properties.First().Key.Should().Be("$set_once");
            properties.First().Value.Should().BeEquivalentTo(new Dictionary<string, object>() { { "key", new [] { 1, 2, 3 } } });
        }

        [TestMethod]
        public void PropertiesTests_SetEventProperty_ShouldAllowOverridingValuesWithSameKey()
        {
            var properties = new Properties().SetUserPopertyOnce("key", new[] { 1, 2, 3 }).SetUserPopertyOnce("key", 1);

            properties.Should().HaveCount(1);
            properties.Should().ContainKey("$set_once");
            properties.First().Value.Should().BeEquivalentTo(new Dictionary<string, object>() { { "key", 1 } });
        }
    }
}