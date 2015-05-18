// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class HighlightTests
    {
        [Fact]
        public void ConstructorHasSensibleDefaultValues()
        {
            var highlight = new Highlight();

            Assert.Null(highlight.PreTag);
            Assert.Null(highlight.PostTag);
            Assert.Empty(highlight.Fields);
        }

        [Fact]
        public void AddFieldsAddsFields()
        {
            var highlight = new Highlight();

            highlight.AddFields("field1", "field2");

            Assert.Contains("field1", highlight.Fields);
            Assert.Contains("field2", highlight.Fields);
            Assert.Equal(2, highlight.Fields.Count);
        }

        [Fact]
        public void PreTagAndPostTagCanBeSet()
        {
            var highlight = new Highlight { PreTag = "Pre", PostTag = "Post" };

            Assert.Equal("Pre", highlight.PreTag);
            Assert.Equal("Post", highlight.PostTag);
        }
    }
}