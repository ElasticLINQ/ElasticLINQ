// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria to select documents if they do not have a value
    /// in the specified field.
    /// </summary>
    internal class MissingCriteria : SingleFieldCriteria, INegatableCriteria
    {
        public MissingCriteria(string field)
            : base(field)
        {
        }

        public override string Name
        {
            get { return "missing"; }
        }

        public ICriteria Negate()
        {
            return new ExistsCriteria(Field);
        }
    }
}