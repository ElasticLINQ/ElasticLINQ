// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using ElasticLinq.Mapping;
using ElasticLinq.Request.Criteria;
using ElasticLinq.Request.Expressions;
using ElasticLinq.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticLinq.Request.Visitors
{
    enum CriteriaWithin { Query, Filter };

    /// <summary>
    /// Expression visitor to translate predicate expressions to criteria expressions.
    /// Used by Where, Query, Single, First, Count etc.
    /// </summary>
    internal abstract class CriteriaExpressionVisitor : ExpressionVisitor
    {
        protected readonly IElasticMapping Mapping;
        protected readonly string Prefix;

        /// <summary>
        /// Whether we are currently within a Where or Query statement as different
        /// operations are permitted in each.
        /// </summary>
        protected CriteriaWithin Within { get; set; }

        /// <summary>
        /// Creates a new CriteriaExpressionVisitor with a given mapping and prefix.
        /// </summary>
        /// <param name="mapping">The IElasticMapping used to translate properties to fields.</param>
        /// <param name="prefix">The string prefix used to prepend fields</param>
        protected CriteriaExpressionVisitor(IElasticMapping mapping, string prefix)
        {
            Mapping = new ElasticFieldsMappingWrapper(mapping);
            Prefix = prefix;
            Within = CriteriaWithin.Filter;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(string))
                return VisitStringMethodCall(m);

            if (m.Method.DeclaringType == typeof(Enumerable))
                return VisitEnumerableMethodCall(m);

            if (m.Method.DeclaringType == typeof(ElasticMethods))
                return VisitElasticMethodsMethodCall(m);

            return VisitDefaultMethodCall(m);
        }

        private Expression VisitDefaultMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Equals":
                    if (m.Arguments.Count == 1)
                        return VisitEquals(Visit(m.Object), Visit(m.Arguments[0]));
                    if (m.Arguments.Count == 2)
                        return VisitEquals(Visit(m.Arguments[0]), Visit(m.Arguments[1]));
                    break;

                case "Contains":
                    if (TypeHelper.FindIEnumerable(m.Method.DeclaringType) != null)
                        return VisitEnumerableContainsMethodCall(m.Object, m.Arguments[0]);
                    break;
            }

            return base.VisitMethodCall(m);
        }

        protected static ICriteria ApplyCriteria(ICriteria currentRoot, ICriteria newCriteria)
        {
            return currentRoot == null
                ? newCriteria
                : AndCriteria.Combine(currentRoot, newCriteria);
        }

        protected Expression VisitElasticMethodsMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "ContainsAny":
                    if (m.Arguments.Count == 2)
                        return VisitContains("ContainsAny", m.Arguments[0], m.Arguments[1], TermsExecutionMode.@bool);
                    break;

                case "ContainsAll":
                    if (m.Arguments.Count == 2)
                        return VisitContains("ContainsAll", m.Arguments[0], m.Arguments[1], TermsExecutionMode.and);
                    break;

                case "Regexp":
                    if (m.Arguments.Count == 2)
                        return VisitRegexp(m.Arguments[0], m.Arguments[1]);
                    break;

                case "Prefix":
                    if (m.Arguments.Count == 2)
                        return VisitPrefix(m.Arguments[0], m.Arguments[1]);
                    break;
            }

            throw new NotSupportedException(string.Format("The ElasticMethods method '{0}' is not supported", m.Method.Name));
        }

        protected Expression VisitEnumerableMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":
                    if (m.Arguments.Count == 2)
                        return VisitEnumerableContainsMethodCall(m.Arguments[0], m.Arguments[1]);
                    break;
            }

            throw new NotSupportedException(string.Format("The Enumerable method '{0}' is not supported", m.Method.Name));
        }

        protected Expression VisitStringMethodCall(MethodCallExpression m)
        {
            switch (m.Method.Name)
            {
                case "Contains":  // Where(x => x.StringProperty.Contains(value))
                    if (m.Arguments.Count == 1)
                        return VisitStringPatternCheckMethodCall(m.Object, m.Arguments[0], "*{0}*", m.Method.Name);
                    break;

                case "StartsWith": // Where(x => x.StringProperty.StartsWith(value))
                    if (m.Arguments.Count == 1)
                        return VisitStringPatternCheckMethodCall(m.Object, m.Arguments[0], "{0}*", m.Method.Name);
                    break;

                case "EndsWith": // Where(x => x.StringProperty.EndsWith(value))
                    if (m.Arguments.Count == 1)
                        return VisitStringPatternCheckMethodCall(m.Object, m.Arguments[0], "*{0}", m.Method.Name);
                    break;
            }

            return VisitDefaultMethodCall(m);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    return node.Operand;

                case ExpressionType.Not:
                    {
                        var subExpression = Visit(node.Operand) as CriteriaExpression;
                        if (subExpression != null)
                            return new CriteriaExpression(NotCriteria.Create(subExpression.Criteria));
                        break;
                    }
            }

            return base.VisitUnary(node);
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(ElasticFields))
                return m;

            switch (m.Expression.NodeType)
            {
                case ExpressionType.Parameter:
                case ExpressionType.MemberAccess:
                    return m;

                default:
                    throw new NotSupportedException(string.Format("The MemberInfo '{0}' is not supported", m.Member.Name));
            }
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.OrElse:
                    return VisitOrElse(b);

                case ExpressionType.AndAlso:
                    return VisitAndAlso(b);

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return VisitComparisonBinary(b);

                default:
                    throw new NotSupportedException(string.Format("The binary expression '{0}' is not supported", b.NodeType));
            }
        }

        protected Expression BooleanMemberAccessBecomesEquals(Expression e)
        {
            if (e is CriteriaExpression)
                return e;

            e = Visit(e);

            var wasNegative = e.NodeType == ExpressionType.Not;

            if (e is UnaryExpression)
                e = Visit(((UnaryExpression)e).Operand);

            if (e is MemberExpression && e.Type == typeof(bool))
                return Visit(Expression.Equal(e, Expression.Constant(!wasNegative)));

            return e;
        }

        private Expression VisitPrefix(Expression fieldExpression, Expression startsWithExpression)
        {
            // Do not use ConstantMemberPair - these expressions are not reversible
            if (fieldExpression is MemberExpression && startsWithExpression is ConstantExpression)
            {
                var fieldName = Mapping.GetFieldName(Prefix, (MemberExpression) fieldExpression);
                return new CriteriaExpression(new PrefixCriteria(fieldName, ((ConstantExpression)startsWithExpression).Value.ToString()));
            }

            throw new NotSupportedException("ElasticMethods.Prefix must take a member for field and a constant for startsWith");
        }

        private Expression VisitRegexp(Expression fieldExpression, Expression regexpExpression)
        {
            // Do not use ConstantMemberPair - these expressions are not reversible
            if (fieldExpression is MemberExpression && regexpExpression is ConstantExpression)
            {
                var fieldName = Mapping.GetFieldName(Prefix, (MemberExpression)fieldExpression);
                return new CriteriaExpression(new RegexpCriteria(fieldName, ((ConstantExpression)regexpExpression).Value.ToString()));
            }

            throw new NotSupportedException("ElasticMethods.Regexp must take a member for field and a constant for startsWith");
        }

        private Expression VisitEnumerableContainsMethodCall(Expression source, Expression match)
        {
            var matched = Visit(match);

            // Where(x => constantsList.Contains(x.Property))
            if (source is ConstantExpression && matched is MemberExpression)
            {
                var memberExpression = (MemberExpression)matched;
                var field = Mapping.GetFieldName(Prefix, memberExpression);
                var containsSource = ((IEnumerable)((ConstantExpression)source).Value);

                // If criteria contains a null create an Or criteria with Terms on one
                // side and Missing on the other.
                var values = containsSource.Cast<object>().Distinct().ToList();
                var nonNullValues = values.Where(v => v != null).ToList();

                ICriteria criteria = TermsCriteria.Build(field, memberExpression.Member, nonNullValues);
                if (values.Count != nonNullValues.Count)
                    criteria = OrCriteria.Combine(criteria, new MissingCriteria(field));

                return new CriteriaExpression(criteria);
            }

            // Where(x => x.SomeList.Contains(constantValue))
            if (source is MemberExpression && matched is ConstantExpression)
            {
                var memberExpression = (MemberExpression)source;
                var field = Mapping.GetFieldName(Prefix, memberExpression);
                var value = ((ConstantExpression)matched).Value;
                return new CriteriaExpression(TermsCriteria.Build(field, memberExpression.Member, value));
            }

            throw new NotSupportedException(source is MemberExpression
                ? string.Format("Match '{0}' in Contains operation must be a constant", match)
                : string.Format("Unknown source '{0}' for Contains operation", source));
        }

        private Expression VisitStringPatternCheckMethodCall(Expression source, Expression match, string pattern, string methodName)
        {
            if (Within != CriteriaWithin.Query)
                throw new NotSupportedException(string.Format("Method String.{0} can only be used within .Query() not in .Where()", methodName));

            var matched = Visit(match);

            if (source is MemberExpression && matched is ConstantExpression)
            {
                var field = Mapping.GetFieldName(Prefix, (MemberExpression)source);
                var value = ((ConstantExpression)matched).Value;
                return new CriteriaExpression(new QueryStringCriteria(String.Format(pattern, value), field));
            }

            throw new NotSupportedException(source is MemberExpression
                ? string.Format("Match '{0}' in Contains operation must be a constant", match)
                : string.Format("Unknown source '{0}' for Contains operation", source));
        }

        private Expression VisitAndAlso(BinaryExpression b)
        {
            return new CriteriaExpression(
                AndCriteria.Combine(AssertExpressionsOfType<CriteriaExpression>(b.Left, b.Right).Select(f => f.Criteria).ToArray()));
        }

        private Expression VisitOrElse(BinaryExpression b)
        {
            return new CriteriaExpression(
                OrCriteria.Combine(AssertExpressionsOfType<CriteriaExpression>(b.Left, b.Right).Select(f => f.Criteria).ToArray()));
        }

        private IEnumerable<T> AssertExpressionsOfType<T>(params Expression[] expressions) where T : Expression
        {
            foreach (var expression in expressions.Select(BooleanMemberAccessBecomesEquals))
            {
                if ((expression as T) == null)
                    throw new NotSupportedException(string.Format("Unexpected binary expression '{0}'", expression));

                yield return (T)expression;
            }
        }

        private Expression VisitComparisonBinary(BinaryExpression b)
        {
            var left = Visit(b.Left);
            var right = Visit(b.Right);

            switch (b.NodeType)
            {
                case ExpressionType.Equal:
                    return VisitEquals(left, right);

                case ExpressionType.NotEqual:
                    return VisitNotEqual(left, right);

                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    return VisitRange(b.NodeType, left, right);

                default:
                    throw new NotSupportedException(string.Format("The binary expression '{0}' is not supported", b.NodeType));
            }
        }

        private Expression VisitContains(string methodName, Expression left, Expression right, TermsExecutionMode executionMode)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
            {
                var values = ((IEnumerable)cm.ConstantExpression.Value).Cast<object>().ToArray();
                return new CriteriaExpression(TermsCriteria.Build(executionMode, Mapping.GetFieldName(Prefix, cm.MemberExpression), cm.MemberExpression.Member, values));
            }

            throw new NotSupportedException(methodName + " must be between a Member and a Constant");
        }

        private Expression CreateExists(ConstantMemberPair cm, bool positiveTest)
        {
            var fieldName = Mapping.GetFieldName(Prefix, UnwrapNullableMethodExpression(cm.MemberExpression));

            var value = cm.ConstantExpression.Value ?? false;

            if (value.Equals(positiveTest))
                return new CriteriaExpression(new ExistsCriteria(fieldName));

            if (value.Equals(!positiveTest))
                return new CriteriaExpression(new MissingCriteria(fieldName));

            throw new NotSupportedException("A null test Expression must have a member being compared to a bool or null");
        }

        private Expression VisitEquals(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return cm.IsNullTest
                    ? CreateExists(cm, true)
                    : new CriteriaExpression(new TermCriteria(Mapping.GetFieldName(Prefix, cm.MemberExpression), cm.MemberExpression.Member, cm.ConstantExpression.Value));

            throw new NotSupportedException("Equality must be between a Member and a Constant");
        }

        private static MemberExpression UnwrapNullableMethodExpression(MemberExpression m)
        {
            var lhsMemberExpression = m.Expression as MemberExpression;
            if (lhsMemberExpression != null && m.Member.Name == "HasValue" && m.Member.DeclaringType.IsGenericOf(typeof(Nullable<>)))
                return lhsMemberExpression;

            return m;
        }

        private Expression VisitNotEqual(Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
                return cm.IsNullTest
                    ? CreateExists(cm, false)
                    : new CriteriaExpression(NotCriteria.Create(new TermCriteria(Mapping.GetFieldName(Prefix, cm.MemberExpression), cm.MemberExpression.Member, cm.ConstantExpression.Value)));

            throw new NotSupportedException("A not-equal expression must be between a constant and a member");
        }

        private Expression VisitRange(ExpressionType t, Expression left, Expression right)
        {
            var cm = ConstantMemberPair.Create(left, right);

            if (cm != null)
            {
                var field = Mapping.GetFieldName(Prefix, cm.MemberExpression);
                return new CriteriaExpression(new RangeCriteria(field, cm.MemberExpression.Member, ExpressionTypeToRangeType(t), cm.ConstantExpression.Value));
            }

            throw new NotSupportedException("A range must consist of a constant and a member");
        }

        private static RangeComparison ExpressionTypeToRangeType(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.GreaterThan:
                    return RangeComparison.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return RangeComparison.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return RangeComparison.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return RangeComparison.LessThanOrEqual;
            }

            throw new ArgumentOutOfRangeException("expressionType");
        }
    }
}
