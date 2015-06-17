// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that specifies a regular expression must be matched against a field.
    /// </summary>
    public class RegexpCriteria : SingleFieldCriteria
    {
        private readonly string regexp;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexpCriteria"/> class.
        /// </summary>
        /// <param name="field">Field to test the regex against.</param>
        /// <param name="regexp">Regular expression to test against the field.</param>
        public RegexpCriteria(string field, string regexp)
            : base(field)
        {
            this.regexp = regexp;
        }

        /// <summary>
        /// Regular expression (in Elasticsearch syntax) to test against the field.
        /// </summary>
        public string Regexp
        {
            get { return regexp; }
        }

        /// <inheritdoc/>
        public override string Name
        {
            get { return "regexp"; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + "\"" + Regexp + "\"";
        }
    }
}