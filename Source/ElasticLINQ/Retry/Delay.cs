// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Retry
{
    /// <summary>
    /// Implements an asynchronous delay. Replaceable for testing purposes (so unit tests don't actually wait).
    /// </summary>
    public class Delay
    {
        public static readonly Delay Instance = new Delay();

        public virtual Task For(int milliseconds, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Delay(milliseconds, cancellationToken);
        }
    }
}