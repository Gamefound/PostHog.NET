using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostHog.Model;
using System.Collections.Generic;
using System.Linq;

namespace Test.Model
{
    [TestClass]
    public class PropertiesTests_SetUserProperty_ShouldHaveValidValue
    {
        [TestMethod]
        public void PropertiesTests_SetEventProperty_ShouldAddEventProperty()
        {
            var properties = new Properties().SetUserProperty("key", 1);

            properties.Should().HaveCount(1);
            properties.First().Key.Should().Be("$set");
            properties.First().Value.Should().BeEquivalentTo(new Dictionary<string, object>() { { "key", 1 } });
        }
    }
}