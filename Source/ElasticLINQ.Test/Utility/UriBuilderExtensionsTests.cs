// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ElasticLinq.Test.Utility
{
    public class UriBuilderExtensionsTests
    {
        [Fact]
        public void GetQueryParametersReturnsEmptyDictionaryWhenNoParameters()
        {
            var builder = new UriBuilder("http://has.no.query/string");

            var parameters = builder.GetQueryParameters();

            Assert.Empty(parameters.Keys);
        }

        [Fact]
        public void GetQueryParametersReturnsOneEntryWhenOneParameter()
        {
            var builder = new UriBuilder("http://has.no.query/string?key=value");

            var parameters = builder.GetQueryParameters();

            Assert.Equal(1, parameters.Keys.Count);
            Assert.Equal("key", parameters.First().Key);
            Assert.Equal("value", parameters.First().Value);
        }

        [Fact]
        public void GetQueryParametersReturnsTwoEntriesWhenTwoParameters()
        {
            var builder = new UriBuilder("http://has.no.query/string?first=1st&second=2nd");

            var parameters = builder.GetQueryParameters();

            Assert.Equal(2, parameters.Keys.Count);
            Assert.Single(parameters, p => p.Key == "first" && p.Value == "1st");
            Assert.Single(parameters, p => p.Key == "second" && p.Value == "2nd");
        }

        [Fact]
        public void GetQueryParametersReturnsKeysWithBlankValues()
        {
            var builder = new UriBuilder("http://has.no.query/string?first=1st&second");

            var parameters = builder.GetQueryParameters();

            Assert.Equal(2, parameters.Keys.Count);
            Assert.Single(parameters, p => p.Key == "first" && p.Value == "1st");
            Assert.Single(parameters, p => p.Key == "second" && p.Value == "");
        }

        [Fact]
        public void SetQueryParametersBlanksQueryWithNoParameters()
        {
            var builder = new UriBuilder("http://has.no.query/string?first=1st&second");

            builder.SetQueryParameters(new Dictionary<string, string>());

            Assert.Equal("", builder.Query);
        }


        [Fact]
        public void SetQueryParametersSetsQueryWithSingleParameter()
        {
            var builder = new UriBuilder("http://has.no.query/string?something");

            builder.SetQueryParameters(new Dictionary<string, string> { { "first", "1st" } });

            Assert.Equal("http://has.no.query/string?first=1st", builder.Uri.ToString());
        }

        [Fact]
        public void SetQueryParametersSetsQueryWithTwoParameters()
        {
            var builder = new UriBuilder("http://has.no.query/string?something");

            builder.SetQueryParameters(new Dictionary<string, string> { { "first", "1st" }, { "second", "2nd" } });

            Assert.Equal("?first=1st&second=2nd", builder.Query);
        }

        [Fact]
        public void SetQueryParametersSetsQueryWithEmptyValueParameters()
        {
            var builder = new UriBuilder("http://has.no.query/string?something");

            builder.SetQueryParameters(new Dictionary<string, string> { { "first", "" }, { "second", "2nd" } });

            Assert.Equal("?first&second=2nd", builder.Query);
        }
    }
}