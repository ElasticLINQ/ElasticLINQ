using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLinq.Request.Criteria
{
    /// <summary>
    /// Criteria to hold crosstype field call. Need when you must call field of arrayType field on elastic (doc.docTypesArray.typeName)
    /// </summary>
    internal class FieldCallCriteria : ITermsCriteria
    {
        private readonly string _field;
        private readonly MemberInfo _memberInfo;
        private readonly ITermsCriteria _criteria;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="memberInfo"></param>
        /// <param name="criteria"></param>
        public FieldCallCriteria(string field, MemberInfo memberInfo, ITermsCriteria criteria)
        {
            _field = field;
            _memberInfo = memberInfo;
            _criteria = criteria;
        }

        public string Name
        {
            get { return _criteria.Name; }
        }

        public string Field
        {
            get { return _field; }
        }

        public string[] FieldCallHierarchy
        {
            get
            {
                var hierarchy = (new string[] {_field});
                if (_criteria is FieldCallCriteria)
                {
                    hierarchy = hierarchy.Concat((_criteria as FieldCallCriteria).FieldCallHierarchy).ToArray();
                }
                else
                {
                    hierarchy = hierarchy.Concat(new string[] { _criteria.Field }).ToArray();
                }
                return hierarchy;
            }
        }

        public bool IsOrCriteria
        {
            get { return _criteria.IsOrCriteria; }
        }

        public System.Reflection.MemberInfo Member
        {
            get { return _memberInfo; }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<object> Values
        {
            get { return _criteria.Values; }
        }
    }
}
