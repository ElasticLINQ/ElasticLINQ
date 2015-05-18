using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticLinq.Utility
{
    public class HighlightConfig
    {
        public String PreTag { get; set; }
        public String PostTag { get; set; }

        private readonly List<string> _fields;
        public HighlightConfig()
        {
            this._fields = new List<string>();
        }

        internal void AddField(String field)
        {
            _fields.Add(field);
        }

        internal void AddFieldRange(params String[] fields)
        {
            _fields.AddRange(fields);
        }

        public ReadOnlyCollection<string> Fields
        {
            get { return new ReadOnlyCollection<string>(_fields); }
        }
    }
}
