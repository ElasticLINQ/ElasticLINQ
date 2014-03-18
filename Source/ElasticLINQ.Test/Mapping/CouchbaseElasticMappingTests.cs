// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using System;
using Xunit;

namespace ElasticLinq.Test.Mapping
{
    public class CouchbaseElasticMappingTests
    {
        [Fact]
        public static void GetDocumentMappingPrefixReturnsDoc()
        {
            var mapping = new CouchbaseElasticMapping();

            var result = mapping.GetDocumentMappingPrefix(null);

            Assert.Equal("doc", result);
        }

        [Fact]
        public static void GetDocumentTypeIsCouchbaseDocument()
        {
            var mapping = new CouchbaseElasticMapping();

            var result = mapping.GetDocumentType(null);

            Assert.Equal("couchbaseDocument", result);
        }
    }
}