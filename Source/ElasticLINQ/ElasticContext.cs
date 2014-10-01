// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

namespace ElasticLinq
{
    using System;
    using System.Linq;
    using ElasticLinq.Communication.Requests;
    using ElasticLinq.Communication.Responses;
    using ElasticLinq.Logging;
    using ElasticLinq.Mapping;
    using ElasticLinq.Retry;
    using ElasticLinq.Utility;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides an entry point to easily create LINQ queries for Elasticsearch.
    /// </summary>
    public class ElasticContext : IElasticContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticContext"/> class.
        /// </summary>
        /// <param name="connection">The information on how to connect to the Elasticsearch server.</param>
        /// <param name="mapping">The object that helps map queries (optional, defaults to <see cref="TrivialElasticMapping"/>).</param>
        /// <param name="log">The object which logs information (optional, defaults to <see cref="NullLog"/>).</param>
        /// <param name="retryPolicy">The object which controls retry policy for the search (optional, defaults to <see cref="RetryPolicy"/>).</param>
        public ElasticContext(IElasticConnection connection, IElasticMapping mapping = null, ILog log = null, IRetryPolicy retryPolicy = null)
        {
            Argument.EnsureNotNull("connection", connection);

            Connection = connection;
            Mapping = mapping ?? new TrivialElasticMapping();
            Log = log ?? NullLog.Instance;
            RetryPolicy = retryPolicy ?? new RetryPolicy(Log);
        }

        /// <summary>
        /// Specifies the connection to the Elasticsearch server.
        /// </summary>
        public IElasticConnection Connection { get; private set; }

        /// <summary>
        /// The logging mechanism for diagnostics information.
        /// </summary>
        public ILog Log { get; private set; }

        /// <summary>
        /// The mapping to describe how objects and their properties are mapped to Elasticsearch.
        /// </summary>
        public IElasticMapping Mapping { get; private set; }

        /// <summary>
        /// The retry policy for handling networking issues.
        /// </summary>
        public IRetryPolicy RetryPolicy { get; private set; }

        /// <inheritdoc/>
        public virtual IQueryable<T> Query<T>(string indexPath, string typePath)
        {
            var prefix = Mapping.GetDocumentMappingPrefix(typeof(T));
            var provider = new ElasticQueryProvider(Connection, Mapping, Log, RetryPolicy, prefix);
            return new ElasticQuery<T>(provider);
        }

        //public T Get<T>(ElasticIndexPath indexPath, ElasticTypePath typePath, int id)
        //{
        //    return this.Get<T>(indexPath, typePath, id.ToString());
        //}

        public T Get<T>(string indexPath, string typePath, string id)
        {
            var request = new GetRequest
            {
                Index = indexPath,
                Type = typePath,
                Id = id
            };

            var response = AsyncHelper.RunSync(() => this.Connection.Get<GetResponse, GetRequest>(request, this.Log));

            return response.Source.ToObject<T>();
        }

        public void Post<T>(string indexPath, string typePath, T doc)
        {
            var request = new PostRequest
            {
                Index = indexPath,
                Type = typePath
            };

            var response = AsyncHelper.RunSync(() => this.Connection.Post<PostResponse, PostRequest>(request, JsonConvert.SerializeObject(doc), this.Log));
        }

        public void Put<T>(string indexPath, string typePath, T doc, Func<T, string> idExtractor)
        {
            var request = new PutRequest
            {
                Index = indexPath,
                Type = typePath,
                Id = idExtractor(doc)
            };

            var response = AsyncHelper.RunSync(() => this.Connection.Put<PutResponse, PutRequest>(request, JsonConvert.SerializeObject(doc), this.Log));
        }

        public void Update<T>(string indexPath, string typePath, T doc, Func<T, string> idExtractor)
        {
            var request = new UpdateRequest
            {
                Index = indexPath,
                Type = typePath,
                Id = idExtractor(doc)
            };

            var response = AsyncHelper.RunSync(() => this.Connection.Post<UpdateResponse, UpdateRequest>(request, string.Format("{{ \"doc\" : {0} }}", JsonConvert.SerializeObject(doc)), this.Log));
        }

        public void Delete(string indexPath, string typePath, string id)
        {
            var request = new DeleteRequest
            {
                Index = indexPath,
                Type = typePath,
                Id = id
            };

            var response = AsyncHelper.RunSync(() => this.Connection.Delete<DeleteResponse, DeleteRequest>(request, this.Log));
        }

        public virtual bool IndexExists(string indexPath)
        {
            var request = new IndexExistsRequest
            {
                Index = indexPath
            };

            return AsyncHelper.RunSync(() => this.Connection.Head(request, this.Log));
        }

        public virtual bool TypeExists(string indexPath, string typePath)
        {
            var request = new TypeExistsRequest
            {
                Index = indexPath,
                Type = typePath
            };

            return AsyncHelper.RunSync(() => this.Connection.Head(request, this.Log));
        }
    }
}