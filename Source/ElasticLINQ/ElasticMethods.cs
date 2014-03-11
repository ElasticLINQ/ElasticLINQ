// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;

namespace ElasticLinq
{
    /// <summary>
    /// Provides methods that stand-in for special operations in ElasticSearch.
    /// </summary>
    public static class ElasticMethods
    {
        private static readonly Exception exception = new InvalidOperationException("This method is for mapping queries to ElasticSearch and should not be called directly.");

        public static bool ContainsAny<T>(IEnumerable<T> source, IEnumerable<T> items)
        {
            throw exception;
        }

        public static bool ContainsAll<T>(IEnumerable<T> source, IEnumerable<T> items)
        {
            throw exception;
        }

        public static bool Regexp(string property, string value)
        {
            throw exception;
        }

        public static bool Prefix(string property, string value)
        {
            throw exception;
        }
    }
}