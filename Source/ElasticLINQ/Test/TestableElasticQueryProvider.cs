// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Async;
using ElasticLinq.Utility;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ElasticLinq.Test
{
    /// <summary>
    /// Provides an <see cref="IQueryProvider"/> that can be used for unit tests.
    /// </summary>
    public class TestableElasticQueryProvider : IQueryProvider, IAsyncQueryExecutor
    {
        readonly TestableElasticContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestableElasticQueryProvider"/> class.
        /// </summary>
        /// <param name="context">The <see cref="TestableElasticContext"/> used to execute the queries.</param>
        public TestableElasticQueryProvider(TestableElasticContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestableElasticQuery<TElement>(context, expression);
        }

        /// <inheritdoc/>
        public IQueryable CreateQuery(Expression expression)
        {
            return CreateQuery<object>(expression);
        }

        /// <inheritdoc/>
        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }

        /// <inheritdoc/>
        public object Execute(Expression expression)
        {
            expression = TestableElasticQueryRewriter.Rewrite(expression);
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        /// <inheritdoc/>
        public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            return (TResult)await ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = new CancellationToken())
        {
            return await Task.Run(() => Execute(expression), cancellationToken).ConfigureAwait(false);
        }

        class TestableElasticQueryRewriter : ExpressionVisitor
        {
            static readonly TestableElasticQueryRewriter instance = new TestableElasticQueryRewriter();

            public static Expression Rewrite(Expression expression)
            {
                Argument.EnsureNotNull(nameof(expression), expression);
                return instance.Visit(expression);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                return node.Member.DeclaringType == typeof(ElasticFields)
                    ? Expression.Convert(Expression.Constant(TypeHelper.CreateDefault(node.Type)), node.Type)
                    : base.VisitMember(node);
            }
        }
    }
}