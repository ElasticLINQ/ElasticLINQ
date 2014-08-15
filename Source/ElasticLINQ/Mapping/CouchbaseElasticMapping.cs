// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Globalization;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Mapping appropriate for use with the Couchbase/Elasticsearch adapter.
    /// </summary>
    public class CouchbaseElasticMapping : ElasticMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseElasticMapping"/> class.
        /// </summary>
        /// <param name="camelCaseFieldNames">Pass <c>true</c> to automatically camel-case field names (for <see cref="ElasticMapping.GetFieldName"/>).</param>
        /// <param name="lowerCaseAnalyzedFieldValues">Pass <c>true</c> to automatically convert field values to lower case (for <see cref="ElasticMapping.FormatValue"/>).</param>
        /// <param name="conversionCulture">The culture to use for the lower-casing, camel-casing, and pluralization operations. If <c>null</c>,
        /// uses <see cref="CultureInfo.CurrentCulture"/>.</param>
        public CouchbaseElasticMapping(bool camelCaseFieldNames = true,
                                       bool lowerCaseAnalyzedFieldValues = true,
                                       CultureInfo conversionCulture = null)
            : base(camelCaseFieldNames, false, false, lowerCaseAnalyzedFieldValues, EnumFormat.String, conversionCulture) { }

        /// <inheritdoc/>
        public override string GetDocumentMappingPrefix(Type type)
        {
            return "doc";
        }

        /// <inheritdoc/>
        public override string GetDocumentType(Type type)
        {
            return "couchbaseDocument";
        }
    }
}