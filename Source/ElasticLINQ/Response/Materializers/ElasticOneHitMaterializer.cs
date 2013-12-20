// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes one ElasticSearch hit throwing necessary exceptions as required to ensure First/Single semantics.
    /// </summary>
    internal class ElasticOneHitMaterializer : IElasticMaterializer
    {
        private readonly Func<Hit, object> itemCreator;
        private readonly Type elementType;
        private readonly bool throwIfMoreThanOne;
        private readonly bool defaultIfNone;

        public ElasticOneHitMaterializer(Func<Hit, object> itemCreator, Type elementType, bool throwIfMoreThanOne, bool defaultIfNone)
        {
            this.itemCreator = itemCreator;
            this.elementType = elementType;
            this.throwIfMoreThanOne = throwIfMoreThanOne;
            this.defaultIfNone = defaultIfNone;
        }

        public object Materialize(ElasticResponse response)
        {
            var enumerator = response.hits.hits.GetEnumerator();

            if (!enumerator.MoveNext())
                if (defaultIfNone)
                    return elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                else
                    throw new InvalidOperationException("Sequence contains no elements");

            var current = enumerator.Current;

            if (throwIfMoreThanOne && enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return itemCreator(current);
        }
    }
}