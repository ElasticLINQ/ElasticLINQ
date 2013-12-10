// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    public class PrefixCriteria : SingleFieldCriteria
    {
        private readonly object prefix;

        public PrefixCriteria(string field, object prefix)
            : base(field)
        {
            this.prefix = prefix;
        }

        public object Prefix
        {
            get { return prefix; }
        }

        public override string Name
        {
            get { return "prefix"; }
        }

        public override string ToString()
        {
            return base.ToString() + "\"" + Prefix + "\"";
        }
    }
}