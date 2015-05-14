using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticLinq.Utility;

namespace ElasticLinq.Request.Criteria
{
    public class HighlightCriteria:ICriteria
    {
        public HighlightConfig Config { get; set; }
        private readonly ReadOnlyCollection<string> fields;
        public HighlightCriteria(HighlightConfig config, params string[] fields)
        {
            Config = config;
            this.fields = new ReadOnlyCollection<string>(fields ?? new string[0]);
        }

        public ReadOnlyCollection<string> Fields
        {
            get { return fields; }
        }
        public string Name
        {
            get { return "highlight"; }
        }
    }
}
