// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using ElasticLinq.Utility;
using System;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes one hit into a CLR object throwing necessary exceptions as required to ensure First/Single semantics.
    /// </summary>
    internal class OneHitElasticMaterializer : IElasticMaterializer
    {
        private readonly Func<Hit, object> projector;
        private readonly Type elementType;
        private readonly bool throwIfMoreThanOne;
        private readonly bool defaultIfNone;

        /// <summary>
        /// Create an instance of the OneHitElasticMaterializer with the given parameters.
        /// </summary>
        /// <param name="projector">A function to turn a hit into a desired CLR object.</param>
        /// <param name="elementType">The type of CLR object being materialized.</param>
        /// <param name="throwIfMoreThanOne">Whether to throw an InvalidOperationException if there are multiple hits.</param>
        /// <param name="defaultIfNone">Whether to throw an InvalidOperationException if there are no hits.</param>
        public OneHitElasticMaterializer(Func<Hit, object> projector, Type elementType, bool throwIfMoreThanOne, bool defaultIfNone)
        {
            this.projector = projector;
            this.elementType = elementType;
            this.throwIfMoreThanOne = throwIfMoreThanOne;
            this.defaultIfNone = defaultIfNone;
        }

        public object Materialize(ElasticResponse response)
        {
            Argument.EnsureNotNull("response", response);

            using (var enumerator = response.hits.hits.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                    if (defaultIfNone)
                        return TypeHelper.CreateDefault(elementType);
                    else
                        throw new InvalidOperationException("Sequence contains no elements");

                var current = enumerator.Current;

                if (throwIfMoreThanOne && enumerator.MoveNext())
                    throw new InvalidOperationException("Sequence contains more than one element");

                return projector(current);
            }
        }
    }
}