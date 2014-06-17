// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    class MatchAllCriteria : ICriteria
    {
        public string Name
        {
            get { return "match_all"; }
        }
    }
}
