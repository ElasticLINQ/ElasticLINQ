// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    public class RegexpCriteria : SingleFieldCriteria
    {
        private readonly string regexp;

        public RegexpCriteria(string field, string regexp)
            : base(field)
        {
            this.regexp = regexp;
        }

        public string Regexp
        {
            get { return regexp; }
        }

        public override string Name
        {
            get { return "regexp"; }
        }

        public override string ToString()
        {
            return base.ToString() + "\"" + Regexp + "\"";
        }
    }
}