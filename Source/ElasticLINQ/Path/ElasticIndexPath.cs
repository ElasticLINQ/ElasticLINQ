// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Path
{
    using System.Collections.Generic;
    using System.Linq;

    public class ElasticIndexPath
    {
        public IEnumerable<string> IndexNames { get; private set; }

        public ElasticIndexPath(params string[] indexNames)
        {
            this.IndexNames = indexNames;
        }

        public string PathSegment
        {
            get
            {
                if (this.IndexNames.Any())
                {
                    return string.Join(",", this.IndexNames);
                }

                return "_all";
            }
        }
    }
}
