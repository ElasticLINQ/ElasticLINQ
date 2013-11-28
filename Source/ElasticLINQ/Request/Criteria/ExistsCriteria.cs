// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria that selects documents if they have any value
    /// in the specified field.
    /// </summary>
    internal class ExistsCriteria : SingleFieldCriteria, INegatableCriteria
    {
        public ExistsCriteria(string field)
            : base(field)
        {
        }

        public override string Name
        {
            get { return "exists"; }
        }

        public ICriteria Negate()
        {
            return new MissingCriteria(Field);
        }
    }
}