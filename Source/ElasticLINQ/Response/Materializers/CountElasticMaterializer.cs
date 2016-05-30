﻿// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Response.Model;
using System;

namespace ElasticLinq.Response.Materializers
{
    /// <summary>
    /// Materializes a count operation by obtaining the total hits from the response.
    /// </summary>
    class CountElasticMaterializer : IElasticMaterializer
    {
        readonly Type returnType;

        public CountElasticMaterializer(Type returnType)
        {
            this.returnType = returnType;
        }

        /// <summary>
        /// Materialize the result count for a given response.
        /// </summary>
        /// <param name="response">The <see cref="ElasticResponse"/> to obtain the count value from.</param>
        /// <returns>The result count expressed as either an int or long depending on the size of the count.</returns>
        public object Materialize(ElasticResponse response)
        {
            if (response.hits.total < 0)
                throw new ArgumentOutOfRangeException("response", "Contains a negative number of hits.");

            return Convert.ChangeType(response.hits.total, returnType);
        }
    }
}