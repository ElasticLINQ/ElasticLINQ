// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Represents constant placeholders in the criteria tree caused by constant
    /// expressions in where trees until they can be optimized out.
    /// </summary>
    internal sealed class ConstantCriteria : ICriteria
    {
        private readonly object constantValue;

        internal static readonly ConstantCriteria False = new ConstantCriteria(false);
        internal static readonly ConstantCriteria True = new ConstantCriteria(true);

        private ConstantCriteria(object constantValue)
        {
            this.constantValue = constantValue;
        }

        public string Name
        {
            get { return constantValue == null ? "null" : constantValue.ToString(); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}