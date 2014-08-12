// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Request.Criteria;
using ElasticLinq.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Reflection;

namespace ElasticLinq.Mapping
{
    /// <summary>
    /// A base class for mapping Elasticsearch values that can lower-case all field values
    /// (and respects <see cref="NotAnalyzedAttribute"/> to opt-out of the lower-casing), 
    /// camel-case field names, and camel-case and pluralize type names.
    /// </summary>
    public class ElasticMapping : IElasticMapping
    {
        private readonly bool camelCaseFieldNames;
        private readonly bool camelCaseTypeNames;
        private readonly CultureInfo conversionCulture;
        private readonly bool lowerCaseAnalyzedFieldValues;
        private readonly bool pluralizeTypeNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticMapping"/> class.
        /// </summary>
        /// <param name="camelCaseFieldNames">Pass <c>true</c> to automatically camel-case field names (for <see cref="GetFieldName"/>).</param>
        /// <param name="camelCaseTypeNames">Pass <c>true</c> to automatically camel-case type names (for <see cref="GetDocumentType"/>).</param>
        /// <param name="pluralizeTypeNames">Pass <c>true</c> to automatically pluralize type names (for <see cref="GetDocumentType"/>).</param>
        /// <param name="lowerCaseAnalyzedFieldValues">Pass <c>true</c> to automatically convert field values to lower case (for <see cref="FormatValue"/>).</param>
        /// <param name="conversionCulture">The culture to use for the lower-casing, camel-casing, and pluralization operations. If <c>null</c>,
        /// uses <see cref="CultureInfo.CurrentCulture"/>.</param>
        public ElasticMapping(bool camelCaseFieldNames = true,
                              bool camelCaseTypeNames = true,
                              bool pluralizeTypeNames = true,
                              bool lowerCaseAnalyzedFieldValues = true,
                              CultureInfo conversionCulture = null)
        {
            this.camelCaseFieldNames = camelCaseFieldNames;
            this.camelCaseTypeNames = camelCaseTypeNames;
            this.pluralizeTypeNames = pluralizeTypeNames;
            this.lowerCaseAnalyzedFieldValues = lowerCaseAnalyzedFieldValues;
            this.conversionCulture = conversionCulture ?? CultureInfo.CurrentCulture;
        }

        /// <inheritdoc/>
        public virtual JToken FormatValue(MemberInfo member, object value)
        {
            Argument.EnsureNotNull("member", member);

            if (value == null)
                return new JValue((string)null);

            var result = JToken.FromObject(value);
            if (lowerCaseAnalyzedFieldValues && result.Type == JTokenType.String && !IsNotAnalyzed(member))
                result = new JValue(result.Value<string>().ToLower(conversionCulture));

            return result;
        }

        /// <inheritdoc/>
        public virtual string GetFieldName(string prefix, MemberInfo memberInfo)
        {
            Argument.EnsureNotNull("memberInfo", memberInfo);

            var memberName = memberInfo.Name;
            if (camelCaseFieldNames)
                memberName = memberName.ToCamelCase(conversionCulture);

            return String.Format("{0}.{1}", prefix, memberName).TrimStart('.');
        }

        /// <inheritdoc/>
        public virtual string GetDocumentMappingPrefix(Type type)
        {
            return null;
        }

        /// <inheritdoc/>
        public virtual string GetDocumentType(Type type)
        {
            Argument.EnsureNotNull("type", type);

            var result = type.Name;
            if (pluralizeTypeNames)
                result = result.ToPlural(conversionCulture);
            if (camelCaseTypeNames)
                result = result.ToCamelCase(conversionCulture);

            return result;
        }

        /// <inheritdoc/>
        public virtual ICriteria GetTypeExistsCriteria(Type docType)
        {
            Argument.EnsureNotNull("docType", docType);

            // Without any other guidance, we look for the first non-nullable property.
            var prefix = GetDocumentMappingPrefix(docType);
            var fieldName = GetFieldName(prefix, MappingHelpers.GetSelectionProperty(docType));
            return new ExistsCriteria(fieldName);
        }

        /// <summary>
        /// Determine whether a field is "not analyzed". By default, looks for the member to be
        /// decorated with the <see cref="NotAnalyzedAttribute"/>.
        /// </summary>
        /// <param name="member">The member to evaluate.</param>
        /// <returns>Returns <c>true</c> if the field is not analyzed; <c>false</c>, otherwise.</returns>
        protected virtual bool IsNotAnalyzed(MemberInfo member)
        {
            return member.GetCustomAttribute<NotAnalyzedAttribute>(inherit: true) != null;
        }
    }
}
