// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// Interface to describe how types and properties are mapped into ElasticSearch.
    /// </summary>
    public interface IElasticMapping
    {
        /// <summary>
        /// Used to format values used in ElasticSearch criteria. Extending this allows you to
        /// specify rules like lower-casing values for certain types of criteria so that searched
        /// values match the rules ElasticSearch is using to store/search values.
        /// </summary>
        /// <param name="member">The member that this value is searching.</param>
        /// <param name="value">The value to be formatted.</param>
        /// <returns>Returns the formatted value.</returns>
        JToken FormatValue(MemberInfo member, object value);

        /// <summary>
        /// Gets the fully document prefix for a given CLR type. Extending this allows you to change
        /// the mapping of types names into the prefix used when creating ElasticSearch queries against
        /// fields. For example, using the Couchbase/ElasticSearch adapter yield documents with the
        /// prefix "doc", since it wraps all documents into a "doc" object; similarly, developers may
        /// with to "namespace" ElasticSearch documents when using ElasticSearch's auto-schema system,
        /// to prevent type collisions between field with the same name but different document type.
        /// </summary>
        /// <param name="type">The type whose prefix is required.</param>
        /// <returns>Returns the document prefix; may return <c>null</c> or empty string to
        /// indicate that no document prefix is required to search the documents.</returns>
        string GetDocumentMappingPrefix(Type type);

        /// <summary>
        /// Gets the document type name for the given CLR type. Extending this allows you to change the
        /// mapping of types names in the CLR to document type names in ElasticSearch. For example,
        /// using the Couchbase/ElasticSearch adapter yields documents with the document type
        /// "couchbaseDocument", regardless of the CLR type.
        /// </summary>
        /// <param name="type">The type whose name is required.</param>
        /// <returns>Returns the ElasticSearch document type name that matches the type; may
        /// return <c>null</c> or empty string to not limit searches to a document type.</returns>
        string GetDocumentType(Type type);

        /// <summary>
        /// Gets the field name for the given member. Extending this allows you to change the
        /// mapping field names in the CLR to field names in ElasticSearch. Typically, these rules
        /// will need to match the serialization rules you use when storing your documents.
        /// </summary>
        /// <param name="prefix">The prefix to put in front of this field name, if the field is
        /// an ongoing part of the document search.</param>
        /// <param name="memberExpression">The member whose name is required.</param>
        /// <returns>Returns the ElasticSearch field name that matches the member.</returns>
        string GetFieldName(string prefix, MemberExpression memberExpression);

        /// <summary>
        /// Gets criteria that can be used to find documents of a particular type. Will be used by
        /// ElasticLINQ when a query does not have any Where clauses, so that it can unambiguously
        /// find documents of the given type. Typically this should degenerate into an "exists"
        /// test for a field that's known to always have a value.
        /// </summary>
        /// <param name="docType">The type that's being searched.</param>
        /// <returns>The criteria for finding the document.</returns>
        ICriteria GetTypeExistsCriteria(Type docType);
    }
}