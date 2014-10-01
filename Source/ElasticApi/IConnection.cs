namespace ElasticApi
{
    using System.Collections.Generic;

    public interface IConnection
    {
        TResponse Head<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters);
        TResponse Get<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters);
        TResponse Post<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters, object body);
        TResponse Put<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters, object body);
        TResponse Delete<TResponse>(IEnumerable<string> path, IDictionary<string, object> parameters);
    }
}
