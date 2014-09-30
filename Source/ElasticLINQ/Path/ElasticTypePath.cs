// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Path
{
    using System.Collections.Generic;
    using System.Linq;

    public class ElasticTypePath
    {
        public IEnumerable<string> TypeNames { get; private set; }

        public ElasticTypePath(params string[] typeNames)
        {
            this.TypeNames = typeNames;
        }

        public string PathSegment
        {
            get
            {
                if (this.TypeNames.Any())
                {
                    return string.Join(",", this.TypeNames);
                }

                return "*";
            }
        }
    }
}
