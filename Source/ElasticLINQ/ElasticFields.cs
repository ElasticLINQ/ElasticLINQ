// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace ElasticLinq
{
    /// <summary>
    /// Provides properties that stand in for special fields in Elasticsearch.
    /// </summary>
    public static class ElasticFields
    {
        /// <summary>
        /// A property that stands in for the Elasticsearch _score field.
        /// </summary>
        public static double Score
        {
            get { throw BuildException(); } 
        }

        /// <summary>
        /// A property that stands in for the Elasticsearch _id field.
        /// </summary>
        public static string Id
        {
            get { throw BuildException(); }
        }

        /// <summary>
        /// Create the InvalidOperationException fired when trying to access properties of this proxy class.
        /// </summary>
        /// <param name="memberName">Optional name of the member, automatically figured out via CallerMemberName if not specified.</param>
        /// <returns>InvalidOperationException with appropriate error message.</returns>
        static InvalidOperationException BuildException([CallerMemberName] string memberName = null)
        {
            return new InvalidOperationException($"ElasticFields.{memberName} is a property for mapping queries to Elasticsearch and should not be evaluated directly.");
        }
    }
}