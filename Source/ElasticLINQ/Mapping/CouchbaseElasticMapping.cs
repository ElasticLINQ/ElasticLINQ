// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Reflection;
using System.Linq.Expressions;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Mapping appropriate for use with the Couchbase/Elasticsearch adapter.
    /// </summary>
    public class CouchbaseElasticMapping : ElasticMapping
    {
        const string TypeCriteriaMissingExceptionMessage = "Unable to determine document type selection criteria for type '{0}'. " +
                                                                   "Ensure the type has a public read/write property that is non-nullable or marked with the Required attribute.";

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseElasticMapping"/> class.
        /// </summary>
        /// <param name="camelCaseFieldNames">Pass <c>true</c> to automatically camel-case field names (for <see cref="ElasticMapping.GetFieldName(Type, MemberInfo)"/>).</param>
        /// <param name="lowerCaseAnalyzedFieldValues">Pass <c>true</c> to automatically convert field values to lower case (for <see cref="ElasticMapping.FormatValue"/>).</param>
        /// <param name="conversionCulture">The culture to use for the lower-casing, camel-casing, and pluralization operations. If <c>null</c>,
        /// uses <see cref="CultureInfo.CurrentCulture"/>.</param>
        public CouchbaseElasticMapping(bool camelCaseFieldNames = true,
                                       bool lowerCaseAnalyzedFieldValues = true,
                                       CultureInfo conversionCulture = null)
            : base(camelCaseFieldNames, false, false, lowerCaseAnalyzedFieldValues, EnumFormat.String, conversionCulture) { }

        /// <summary>
        /// Gets the fully document prefix for a given CLR type. Extending this allows you to change
        /// the mapping of types names into the prefix used when creating Elasticsearch queries against
        /// fields. For example, using the Couchbase/Elasticsearch adapter yield documents with the
        /// prefix "doc", since it wraps all documents into a "doc" object; similarly, developers may
        /// with to "namespace" Elasticsearch documents when using its auto-schema system,
        /// to prevent type collisions between field with the same name but different document type.
        /// </summary>
        /// <param name="type">The type whose prefix is required.</param>
        /// <returns>Returns the document prefix; may return <c>null</c> or empty string to
        /// indicate that no document prefix is required to search the documents.</returns>
        public virtual string GetDocumentMappingPrefix(Type type)
        {
            return "doc";
        }

        /// <inheritdoc/>
        public override string GetDocumentType(Type type)
        {
            return "couchbaseDocument";
        }

        /// <inheritdoc/>
        public override string GetFieldName(Type type, MemberExpression memberExpression)
        {
            switch (memberExpression.Expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return GetFieldName(type, (MemberExpression)memberExpression.Expression) + "." + GetMemberName(memberExpression.Member);

                case ExpressionType.Parameter:
                    return GetFieldName(type, memberExpression.Member);

                default:
                    throw new NotSupportedException($"Unknown expression type {memberExpression.Expression.NodeType} for left hand side of expression {memberExpression}");
            }
        }

        /// <inheritdoc/>
        public override string GetFieldName(Type type, MemberInfo memberInfo)
        {
            var memberName = base.GetFieldName(type, memberInfo);
            var prefix = GetDocumentMappingPrefix(type);

            return $"{prefix}.{memberName}".TrimStart('.');
        } 

        /// <inheritdoc/>
        public override ICriteria GetTypeSelectionCriteria(Type type)
        {
            var property = MappingHelpers.GetTypeSelectionProperty(type);
            if (property == null)
                throw new InvalidOperationException(string.Format(TypeCriteriaMissingExceptionMessage, type.Name));

            return new ExistsCriteria(GetFieldName(type, property));
        }

        /// <inheritdoc/>
        public override object Materialize(JToken sourceDocument, Type sourceType)
        {
            return base.Materialize(sourceDocument.SelectToken(GetDocumentMappingPrefix(sourceType)), sourceType);
        }
    }
}