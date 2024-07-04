using System.Collections.Generic;
using System.Text.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostHog.Model;

namespace Test.Model;

[TestClass]
public class BatchTests_Serialize
{
    [TestMethod]
    public void BatchTests_Serialize_ShouldBeSerializedProperlyAndDeserialize()
    {
        // arrange
        var batch = new Batch(new List<BaseAction>()
        {
            new Capture("capture", "distinct_id_1")
        }, "api_key");
        
        // act
        var json = JsonSerializer.Serialize(batch, new JsonSerializerOptions());
        
        // assert
        var deserialized = JsonSerializer.Deserialize<Batch>(json);
        deserialized.Should().BeEquivalentTo(batch);
    }
}