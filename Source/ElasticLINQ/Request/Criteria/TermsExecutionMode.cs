// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    public enum TermsExecutionMode
    {
        plain,
        fielddata,
        @bool,
        and,
        or
    }
}
