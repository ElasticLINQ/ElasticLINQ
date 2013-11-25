// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System.IO;
using System.Text;
using ElasticLinq.Request;
using ElasticLinq.Utility;
using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace ElasticLinq.Test.Request
{
    public class ElasticRequestProcessorTests
    {
        private static readonly ElasticConnection connection = new ElasticConnection(new Uri("http://localhost"), TimeSpan.FromSeconds(10));

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorThrowsArgumentNullExceptionWhenConnectionIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ElasticRequestProcessor(null, StreamWriter.Null));
        }

        [Fact]
        [ExcludeFromCodeCoverage] // Expression isn't "executed"
        public void ConstructorDoesntThrowWithValidParameters()
        {
            Assert.DoesNotThrow(() => new ElasticRequestProcessor(connection, StreamWriter.Null));
        }

        [Fact]
        public void ParseResponseReturnsParsedResponseGivenValidStream()
        {
            const int took = 2;
            const int shards = 1;
            const int hits = 1;
            const double score = 0.3141;
            const string index = "testIndex";
            const string type = "testType";
            const string id = "testId";

            var responseString = BuildResponseString(took, shards, hits, score, index, type, id);

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
            {
                var response = ElasticRequestProcessor.ParseResponse(stream, StreamWriter.Null);
                Assert.NotNull(response);
                Assert.Equal(took, response.took);
                Assert.Equal(shards, response._shards.successful);
                Assert.Equal(shards, response._shards.total);
                Assert.Equal(shards, response._shards.successful);
                Assert.Equal(0, response._shards.failed);
                Assert.Equal(hits, response.hits.total);
                Assert.Equal(score, response.hits.max_score);

                Assert.NotEmpty(response.hits.hits);
                Assert.Equal(score, response.hits.hits[0]._score);
                Assert.Equal(index, response.hits.hits[0]._index);
                Assert.Equal(type, response.hits.hits[0]._type);
                Assert.Equal(id, response.hits.hits[0]._id);
            }
        }

        private static string BuildResponseString(int took, int shards, int hits, double score, string index, string type, string id)
        {
            return "{\"took\":" + took + "," +
                                 "\"timed_out\":false," +
                                 "\"_shards\":{\"total\":" + shards + "," +
                                 "\"successful\":" + shards + "" +
                                 ",\"failed\":0}," +
                                 "\"hits\":{\"total\":" + hits + "," +
                                 "\"max_score\":" + score + ",\"hits\":" +
                                 "[{\"_index\":\"" + index + "\"," +
                                 "\"_type\":\"" + type + "\"," +
                                 "\"_id\":\"" + id + "\"," +
                                 "\"_score\":" + score + "," +
                                 "\"fields\":{\"name\":\"testField\"}}]}}";
        }
    }
}