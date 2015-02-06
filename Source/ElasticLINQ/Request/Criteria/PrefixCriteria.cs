// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;

namespace ElasticLinq.Request.Criteria
{
    public class PrefixCriteria : SingleFieldCriteria
    {
        private readonly string prefix;

        public PrefixCriteria(string field, string prefix)
            : base(field)
        {
            this.prefix = prefix;
        }

        public string Prefix
        {
            get { return prefix; }
        }

        public override string Name
        {
            get { return "prefix"; }
        }

        public override string ToString()
        {
            return String.Format("{0}\"{1}\"", base.ToString(), Prefix);
        }
    }
}