// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Retry
{
    public class RetryFailedException : Exception
    {
        public RetryFailedException(int maxAttempts)
            : base(String.Format("The operation did not succeed after the maximum number of retries ({0}).", maxAttempts)) { }
    }
}
