// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;

namespace ElasticLinq
{
    /// <summary>
    /// Provides properties that stand-in for special fields in ElasticSearch.
    /// </summary>
    public static class ElasticFields
    {
        private static readonly Exception exception = new InvalidOperationException("This property is for mapping queries to ElasticSearch and should not be evaluated directly.");

        public static double Score
        {
            get { throw exception; } 
        }

        public static string Id
        {
            get { throw exception; }
        }
    }
}