// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Interface to describe how types and properties are mapped into Elasticsearch.
    /// </summary>
    public interface IElasticMapping
    {
        /// <summary>
        /// Used to format values used in Elasticsearch criteria. Extending this allows you to
        /// specify rules like lower-casing values for certain types of criteria so that searched
        /// values match the rules Elasticsearch is using to store/search values.
        /// </summary>
        /// <param name="member">The member that this value is searching.</param>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>Returns the formatted value.</returns>
        JToken FormatValue(MemberInfo member, object value);

        /// <summary>
        /// Gets the document type name for the given CLR type. Extending this allows you to change the
        /// mapping of types names in the CLR to document type names in Elasticsearch. For example,
        /// using the Couchbase/Elasticsearch adapter yields documents with the document type
        /// "couchbaseDocument", regardless of the CLR type.
        /// </summary>
        /// <param name="type">The CLR type whose name is required.</param>
        /// <returns>Returns the Elasticsearch document type name that matches the type; may
        /// return <c>null</c> or empty string to not limit searches to a document type.</returns>
        string GetDocumentType(Type type);

        /// <summary>
        /// Gets the field name for the given member. Extending this allows you to change the
        /// mapping field names in the CLR to field names in Elasticsearch. Typically, these rules
        /// will need to match the serialization rules you use when storing your documents.
        /// </summary>
        /// <param name="type">The CLR type used in the source query.</param>
        /// <param name="memberExpression">The member expression whose name is required.</param>
        /// <returns>Returns the Elasticsearch field name that matches the member.</returns>
        string GetFieldName(Type type, MemberExpression memberExpression);

        /// <summary>
        /// Gets criteria that can be used to find documents of a particular type. Will be used by
        /// ElasticLINQ when a query does not have any suitable Where or Query criteria, so that it
        /// can unambiguously select documents of the given type. Typically this should return an 
        /// ExistsCriteria for a field that's known to always have a value.
        /// </summary>
        /// <param name="type">The CLR type that's being searched.</param>
        /// <returns>The criteria for selecting documents of this type.</returns>
        ICriteria GetTypeSelectionCriteria(Type type);

        /// <summary>
        /// Materialize the JObject hit object from Elasticsearch to a CLR object.
        /// </summary>
        /// <param name="sourceDocument">JSON source document.</param>
        /// <param name="sourceType">Type of CLR object to materialize to.</param>
        /// <returns>Freshly materialized CLR object version of the source document.</returns>
        object Materialize(JToken sourceDocument, Type sourceType);

        /// <summary>
        /// Get the Elastic field type that corresponds with this CLR type.
        /// </summary>
        /// <param name="type">The CLR type to get the equivalent Elastic field type for.</param>
        /// <returns>The corresponding Elastic field type.</returns>
        string GetElasticFieldType(Type type);
    }
}