// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Diagnostics;

namespace ElasticLinq.Request.Criteria
{
    [DebuggerDisplay("match_all")]
    class MatchAllCriteria : ICriteria
    {
        public static readonly MatchAllCriteria Instance = new MatchAllCriteria();

        private MatchAllCriteria() { }

        public string Name
        {
            get { return "match_all"; }
        }
    }
}