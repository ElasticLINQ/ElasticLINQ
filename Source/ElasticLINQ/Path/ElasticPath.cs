// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq.Path
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class ElasticPath
    {
        public ElasticIndexPath IndexPath { get; private set; }

        public ElasticTypePath TypePath { get; private set; }

        public ElasticPath(ElasticIndexPath indexPath, ElasticTypePath typePath)
        {
            this.IndexPath = indexPath;
            this.TypePath = typePath;
        }

        public static ElasticIndexPath Index(params string[] indexNames)
        {
            return new ElasticIndexPath(indexNames);
        }

        public static ElasticTypePath Type(params string[] typeNames)
        {
            return new ElasticTypePath(typeNames);
        }
    }
}
