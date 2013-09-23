// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IQToolkit
{
#if NOREFEMIT
    public static class ExpressionEvaluator
    {
        public static object Eval(Expression expression)
        {
            LambdaExpression lambda = expression as LambdaExpression;
            if (lambda != null && lambda.Parameters.Count == 0)
            {
                expression = lambda.Body;
            }
            else
            {
                throw new InvalidOperationException("Wrong number of arguments specified");
            }
            var result = EvaluatorBuilder.Build(null, null, expression);
            return result.EvalBoxed(new EvaluatorState(null, null));
        }

        public static object Eval(LambdaExpression function, params object[] args)
        {
            if (function.Parameters.Count != args.Length)
            {
                throw new InvalidOperationException("Wrong number of arguments specified");
            }
            var result = EvaluatorBuilder.Build(null, function.Parameters, function.Body);
            return result.EvalBoxed(new EvaluatorState(null, args));
        }

        public static Delegate CreateDelegate(LambdaExpression function)
        {
            var result = EvaluatorBuilder.Build(null, function.Parameters, function.Body);
            return CreateDelegate(function.Type, null, function.Parameters.Count, result);
        }

        public static D CreateDelegate<D>(Expression<D> function)
        {
            return (D)(object)CreateDelegate((LambdaExpression)function);
        }

        public static Delegate CreateDelegate(Type delegateType, EvaluatorState outer, int nArgs, Evaluator evaluator)
        {
            MethodInfo miInvoke = delegateType.GetMethod("Invoke");
            var host = new EvaluatorHost(outer, nArgs, evaluator);
            return StrongDelegate.CreateDelegate(delegateType, host.Eval);
        }

        public class EvaluatorHost
        {
            EvaluatorState outer;
            int nArgs;
            Evaluator evaluator;


            public EvaluatorHost(EvaluatorState outer, int nArgs, Evaluator evaluator)
            {
                this.outer = outer;
                this.nArgs = nArgs;
                this.evaluator = evaluator;
            }

            public object Eval(object[] args)
            {
                int len = (args != null ? args.Length : 0);
                if (len != this.nArgs)
                {
                    object[] tmp = new object[this.nArgs];
                    if (args != null)
                        Array.Copy(args, tmp, len);
                    args = tmp;
                }
                return this.evaluator.EvalBoxed(new EvaluatorState(this.outer, args));
            }
        }

        public class EvaluatorState
        {
            EvaluatorState outer;
            object[] values;
            int start;

            public EvaluatorState(EvaluatorState outer, object[] values)
            {
                this.outer = outer;
                this.values = values;
                this.start = (outer != null ? outer.start + (outer.values != null ? outer.values.Length : 0) : 0);
            }

            public object GetBoxedValue(int index)
            {
                var state = this;
                while (index < state.start)
                {
                    state = state.outer;
                }
                return state.values[index - state.start];
            }

            public T GetValue<T>(int index)
            {
                var state = this;
                while (index < state.start)
                {
                    state = state.outer;
                }
                return (T)state.values[index - state.start];
            }

            public void SetValue<T>(int index, T value)
            {
                var state = this;
                while (index < state.start)
                {
                    state = state.outer;
                }
                state.values[index - state.start] = value;
            }
        }

        private class EvaluatorBuilder
        {
            EvaluatorBuilder outer;
            ReadOnlyCollection<ParameterExpression> parameters;
            int count = -1;

            private EvaluatorBuilder(EvaluatorBuilder outer, List<ParameterExpression> parameters)
            {
                this.outer = outer;
                this.parameters = parameters.ToReadOnly();
            }

            internal static Evaluator Build(EvaluatorBuilder outer, IEnumerable<ParameterExpression> parameters, Expression expression)
            {
                var list = parameters.ToList();
                list.AddRange(VariableFinder.Find(expression));
                return new EvaluatorBuilder(outer, list).Build(expression);
            }

            private int Count
            {
                get
                {
                    if (this.count == -1)
                    {
                        this.count = (this.outer != null ? this.outer.Count : 0) + (this.parameters != null ? this.parameters.Count : 0);
                    }
                    return this.count;
                }
            }

            // treat invocations with nested lambda's as nested expressions w/ variable declarations
            class VariableFinder : ExpressionVisitor
            {
                List<ParameterExpression> variables = new List<ParameterExpression>();

                internal static List<ParameterExpression> Find(Expression expression)
                {
                    var finder = new VariableFinder();
                    finder.Visit(expression);
                    return finder.variables;
                }

                protected override Expression VisitInvocation(InvocationExpression iv)
                {
                    LambdaExpression lambda = iv.Expression as LambdaExpression;
                    if (lambda != null)
                    {
                        this.variables.AddRange(lambda.Parameters);
                    }
                    return base.VisitInvocation(iv);
                }
            }

            private Evaluator Build(Expression exp)
            {
                if (exp == null)
                    return null;
                switch (exp.NodeType)
                {
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                    case ExpressionType.ArrayLength:
                    case ExpressionType.Quote:
                    case ExpressionType.TypeAs:
                    case ExpressionType.UnaryPlus:
                        return Unary((UnaryExpression)exp);
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.Coalesce:
                    case ExpressionType.ArrayIndex:
                    case ExpressionType.RightShift:
                    case ExpressionType.LeftShift:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.Power:
                        return Binary((BinaryExpression)exp);
                    case ExpressionType.Constant:
                        return Constant((ConstantExpression)exp);
                    case ExpressionType.Parameter:
                        return Parameter((ParameterExpression)exp);
                    case ExpressionType.MemberAccess:
                        return MemberAccess((MemberExpression)exp);
                    case ExpressionType.Call:
                        return Call((MethodCallExpression)exp);
                    case ExpressionType.Conditional:
                        return Conditional((ConditionalExpression)exp);
                    case ExpressionType.TypeIs:
                        return TypeIs((TypeBinaryExpression)exp);
                    case ExpressionType.New:
                        return New((NewExpression)exp);
                    case ExpressionType.Lambda:
                        return Lambda((LambdaExpression)exp);
                    case ExpressionType.NewArrayInit:
                        return NewArrayInit((NewArrayExpression)exp);
                    case ExpressionType.NewArrayBounds:
                        return NewArrayBounds((NewArrayExpression)exp);
                    case ExpressionType.Invoke:
                        return Invoke((InvocationExpression)exp);
                    case ExpressionType.MemberInit:
                        return MemberInit((MemberInitExpression)exp);
                    case ExpressionType.ListInit:
                        return ListInit((ListInitExpression)exp);
                    default:
                        throw new InvalidOperationException();
                }
            }

            private Evaluator Build(Type resultType, Expression expression)
            {
                if (expression.Type != resultType)
                {
                    expression = Expression.Convert(expression, resultType);
                }
                return Build(expression);
            }

            private Evaluator Unary(UnaryExpression u)
            {
                var operand = Build(u.Operand);

                bool isSourceTypeNullable = IsNullable(u.Operand.Type);
                bool isTargetTypeNullable = IsNullable(u.Type);
                Type sourceType = GetNonNullType(u.Operand.Type);
                Type targetType = GetNonNullType(u.Type);

                if (u.Method != null)
                {
                    return this.GetUnaryOperator(u, sourceType, targetType, u.Method, operand);
                }

                switch (u.NodeType)
                {
                    case ExpressionType.Negate:
                    case ExpressionType.NegateChecked:
                    case ExpressionType.Not:
                        {
                            MethodInfo mi = FindBestMethod(Operators.GetOperatorMethods(u.NodeType.ToString()), new Type[] { sourceType }, targetType);
                            return this.GetUnaryOperator(u, sourceType, targetType, mi, operand);
                        }
                    case ExpressionType.Convert:
                    case ExpressionType.ConvertChecked:
                        {
                            if (u.Type == u.Operand.Type)
                            {
                                // no conversion necessary
                                return operand;
                            }
                            else if (!sourceType.IsValueType || !targetType.IsValueType)
                            {
                                // reference or boxing conversion
                                return (Evaluator)Activator.CreateInstance(
                                    typeof(Convert<,>).MakeGenericType(u.Operand.Type, u.Type), 
                                    new object[] { operand }
                                    );
                            }
                            else if (sourceType == targetType)
                            {
                                if (isSourceTypeNullable && !isTargetTypeNullable)
                                {
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(ConvertNtoNN<>).MakeGenericType(sourceType), 
                                        new object[] { operand }
                                        );
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Assert(!isSourceTypeNullable && isTargetTypeNullable);
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(ConvertNNtoN<>).MakeGenericType(sourceType), 
                                        new object[] { operand }
                                        );
                                }
                            }
                            else
                            {
                                MethodInfo mi = FindBestMethod(Operators.GetOperatorMethods(u.NodeType + "To" + targetType.Name), new Type[] { sourceType }, targetType);
                                Delegate fn = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(sourceType, targetType), null, mi);
                                if (!isSourceTypeNullable && !isTargetTypeNullable)
                                {
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(Convert<,>).MakeGenericType(sourceType, targetType), 
                                        new object[] { operand, fn }
                                        );
                                }
                                else if (isSourceTypeNullable && !isTargetTypeNullable)
                                {
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(ConvertNtoNN<,>).MakeGenericType(sourceType, targetType), 
                                        new object[] { operand, fn }
                                        );
                                }
                                else if (!isSourceTypeNullable && isTargetTypeNullable)
                                {
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(ConvertNNtoN<,>).MakeGenericType(sourceType, targetType), 
                                        new object[] { operand, fn }
                                        );
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Assert(isSourceTypeNullable && isTargetTypeNullable);
                                    return (Evaluator)Activator.CreateInstance(
                                        typeof(ConvertNtoN<,>).MakeGenericType(sourceType, targetType), 
                                        new object[] { operand, fn }
                                        );
                                }
                            }
                        }
                    case ExpressionType.TypeAs:
                        return (Evaluator)Activator.CreateInstance(
                            typeof(TypeAsEvaluator<,>).MakeGenericType(u.Operand.Type, u.Type), 
                            new object[] { operand }
                            );
                    case ExpressionType.UnaryPlus:
                        return operand;
                    case ExpressionType.ArrayLength:
                        return (Evaluator)Activator.CreateInstance(
                            typeof(ArrayLengthEvaluator<>).MakeGenericType(u.Operand.Type.GetElementType()), 
                            new object[] { operand }
                            );
                    case ExpressionType.Quote:
                        return Quote(u);
                    default:
                        throw new InvalidOperationException();
                }
            }

            private Evaluator GetUnaryOperator(UnaryExpression u, Type sourceType, Type targetType, MethodInfo method, Evaluator operand)
            {
                Delegate opFunc = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(sourceType, targetType), null, method);
                if (u.IsLiftedToNull)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(LiftToNullUnaryEvaluator<,>).MakeGenericType(sourceType, targetType),
                        new object[] { operand, opFunc }
                        );
                }
                else if (u.IsLifted)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(LiftToFalseUnaryEvaluator<>).MakeGenericType(sourceType),
                        new object[] { operand, opFunc }
                        );
                }
                else
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(UnaryEvaluator<,>).MakeGenericType(sourceType, targetType),
                        new object[] { operand, opFunc }
                        );
                }
            }

            private Evaluator Binary(BinaryExpression b)
            {
                var opLeft = Build(b.Left);
                var opRight = Build(b.Right);

                Type sourceType = GetNonNullType(b.Left.Type);
                Type targetType = GetNonNullType(b.Type);

                if (b.Method != null)
                {
                    return GetBinaryOperator(b, sourceType, targetType, b.Method, opLeft, opRight);
                }

                switch (b.NodeType)
                {
                    case ExpressionType.Add:
                    case ExpressionType.AddChecked:
                    case ExpressionType.Subtract:
                    case ExpressionType.SubtractChecked:
                    case ExpressionType.Multiply:
                    case ExpressionType.MultiplyChecked:
                    case ExpressionType.Divide:
                    case ExpressionType.Modulo:
                    case ExpressionType.And:
                    case ExpressionType.Or:
                    case ExpressionType.LessThan:
                    case ExpressionType.LessThanOrEqual:
                    case ExpressionType.GreaterThan:
                    case ExpressionType.GreaterThanOrEqual:
                    case ExpressionType.Equal:
                    case ExpressionType.NotEqual:
                    case ExpressionType.ExclusiveOr:
                    case ExpressionType.RightShift:
                    case ExpressionType.LeftShift:
                    case ExpressionType.Power:
                        {
                            MethodInfo mi = this.FindBestMethod(
                                Operators.GetOperatorMethods(b.NodeType.ToString()),
                                new Type[] { sourceType, sourceType }, targetType
                                );
                            System.Diagnostics.Debug.Assert(mi != null);
                            return GetBinaryOperator(b, sourceType, targetType, mi, opLeft, opRight);
                        }
                    case ExpressionType.AndAlso:
                        if (b.IsLiftedToNull)
                        {
                            return new LiftToNullAndAlsoEvaluator((Evaluator<bool?>)opLeft, (Evaluator<bool?>)opRight);
                        }
                        else if (b.IsLifted)
                        {
                            return new LiftToFalseAndAlsoEvaluator((Evaluator<bool?>)opLeft, (Evaluator<bool?>)opRight);
                        }
                        else
                        {
                            return new AndAlsoEvaluator((Evaluator<bool>)opLeft, (Evaluator<bool>)opRight);
                        }
                    case ExpressionType.OrElse:
                        if (b.IsLiftedToNull)
                        {
                            return new LiftToNullOrElseEvaluator((Evaluator<bool?>)opLeft, (Evaluator<bool?>)opRight);
                        }
                        else if (b.IsLifted)
                        {
                            return new LiftToFalseOrElseEvaluator((Evaluator<bool?>)opLeft, (Evaluator<bool?>)opRight);
                        }
                        else
                        {
                            return new OrElseEvaluator((Evaluator<bool>)opLeft, (Evaluator<bool>)opRight);
                        }
                    case ExpressionType.Coalesce:
                        Type rightType = GetNonNullType(b.Right.Type);
                        if (b.Conversion != null)
                        {
                            LambdaExpression conv = b.Conversion;
                            if (conv.Body.Type != b.Type)
                            {
                                conv = Expression.Lambda(Expression.Convert(conv.Body, b.Type), conv.Parameters.ToArray());
                            }
                            if (conv.Parameters[0].Type == b.Left.Type || conv.Parameters[0].Type == sourceType)
                            {
                                if (conv.Parameters[0].Type == sourceType)
                                {
                                    var p = Expression.Parameter(b.Left.Type, "left");
                                    conv = Expression.Lambda(Expression.Invoke(conv, Expression.Convert(p, sourceType)), p);
                                }
                                Delegate fnConv = ExpressionEvaluator.CreateDelegate(conv);
                                return (Evaluator)Activator.CreateInstance(
                                    typeof(CoalesceREvaluator<,>).MakeGenericType(b.Left.Type, b.Right.Type), 
                                    new object[] { opLeft, opRight, fnConv }
                                    );
                            }
                            else if (conv.Parameters[0].Type == b.Right.Type || conv.Parameters[0].Type == rightType)
                            {
                                if (conv.Parameters[0].Type == rightType)
                                {
                                    var p = Expression.Parameter(b.Right.Type, "right");
                                    conv = Expression.Lambda(Expression.Invoke(conv, Expression.Convert(p, rightType)), p);
                                }
                                Delegate fnConv = ExpressionEvaluator.CreateDelegate(conv);
                                return (Evaluator)Activator.CreateInstance(
                                    typeof(CoalesceLEvaluator<,>).MakeGenericType(b.Left.Type, b.Right.Type), 
                                    new object[] { opLeft, opRight, fnConv }
                                    );
                            }
                        }
                        else if (b.Type == b.Right.Type)
                        {
                            var p = Expression.Parameter(b.Left.Type, "left");
                            var lambda = Expression.Lambda(Expression.Convert(p, b.Type), p);
                            Delegate fnConv = ExpressionEvaluator.CreateDelegate(lambda);
                            return (Evaluator)Activator.CreateInstance(
                                typeof(CoalesceREvaluator<,>).MakeGenericType(b.Left.Type, b.Right.Type), 
                                new object[] { opLeft, opRight, fnConv }
                                );
                        }
                        else if (b.Type == b.Left.Type)
                        {
                            var p = Expression.Parameter(b.Right.Type, "right");
                            var lambda = Expression.Lambda(Expression.Convert(p, b.Type), p);
                            Delegate fnConv = ExpressionEvaluator.CreateDelegate(lambda);
                            return (Evaluator)Activator.CreateInstance(
                                typeof(CoalesceLEvaluator<,>).MakeGenericType(b.Left.Type, b.Right.Type), 
                                new object[] { opLeft, opRight, fnConv }
                                );
                        }
                        throw new InvalidOperationException("Unhandled Coalesce transaltion");
                    case ExpressionType.ArrayIndex:
                        return (Evaluator)Activator.CreateInstance(
                            typeof(ArrayIndexEvaluator<>).MakeGenericType(b.Left.Type.GetElementType()), 
                            new object[] { opLeft, opRight }
                            );
                    default:
                        throw new InvalidOperationException();
                }
            }

            public Evaluator GetBinaryOperator(BinaryExpression b, Type sourceType, Type targetType, MethodInfo method, Evaluator opLeft, Evaluator opRight)
            {
                Delegate opFunc = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(sourceType, sourceType, targetType), null, method);
                if (b.IsLiftedToNull)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(LiftToNullBinaryEvaluator<,>).MakeGenericType(sourceType, targetType),
                        new object[] { opLeft, opRight, opFunc }
                        );
                }
                else if (b.IsLifted)
                {
                    if (b.NodeType == ExpressionType.Equal)
                    {
                        return (Evaluator)Activator.CreateInstance(
                            typeof(LiftToEqualBinaryEvaluator<>).MakeGenericType(sourceType),
                            new object[] { opLeft, opRight, opFunc }
                            );
                    }
                    else if (b.NodeType == ExpressionType.NotEqual)
                    {
                        return (Evaluator)Activator.CreateInstance(
                            typeof(LiftToNotEqualBinaryEvaluator<>).MakeGenericType(sourceType),
                            new object[] { opLeft, opRight, opFunc }
                            );
                    }
                    else
                    {
                        return (Evaluator)Activator.CreateInstance(
                            typeof(LiftToFalseBinaryEvaluator<>).MakeGenericType(sourceType),
                            new object[] { opLeft, opRight, opFunc }
                            );
                    }
                }
                else
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(BinaryEvaluator<,>).MakeGenericType(sourceType, targetType),
                        new object[] { opLeft, opRight, opFunc }
                        );
                }
            }

            public Evaluator Constant(ConstantExpression c)
            {
                return (Evaluator)Activator.CreateInstance(
                    typeof(ConstantEvaluator<>).MakeGenericType(c.Type), 
                    new object[] { c.Value }
                    );
            }

            public Evaluator Parameter(ParameterExpression p)
            {
                int index = this.FindParameterIndex(p);
                return (Evaluator)Activator.CreateInstance(
                    typeof(ParameterEvaluator<>).MakeGenericType(p.Type), 
                    new object[] { index }
                    );
            }

            private int FindParameterIndex(ParameterExpression p)
            {
                for (var builder = this; builder != null; builder = builder.outer)
                {
                    if (this.parameters != null)
                    {
                        for (int i = 0, n = this.parameters.Count; i < n; i++)
                        {
                            if (this.parameters[i] == p)
                            {
                                return i + (this.outer != null ? this.outer.Count : 0);
                            }
                        }
                    }
                }
                throw new InvalidOperationException(string.Format("Parameter '{0}' not in scope", p.Name));
            }

            public Evaluator Call(MethodCallExpression mc)
            {
                var opInst = (mc.Object != null) ? Build(mc.Object) : null;
                var opArgs = mc.Arguments.Select(a => Build(a)).ToArray();
                return this.GetMethodCallOperator(mc.Method, opInst, opArgs);
            }

            public Evaluator GetMethodCallOperator(MethodInfo method, Evaluator opInst, Evaluator[] opArgs)
            {
                var parameters = method.GetParameters();
                if (parameters.Any(p => p.ParameterType.IsByRef))
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(MethodCallWithRefArgsEvaluator<>).MakeGenericType(method.ReturnType),
                        new object[] { method, opInst, opArgs }
                        );
                }
                else if (method.IsStatic && opArgs.Length == 1)
                {
                    var types = new Type[] { parameters[0].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,>).MakeGenericType(types),
                        new object[] { opArgs[0], fn }
                        );
                }
                else if (!method.IsStatic && opArgs.Length == 0)
                {
                    var types = new Type[] { opInst.ReturnType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,>).MakeGenericType(types),
                        new object[] { opInst, fn }
                        );
                }
                else if (method.IsStatic && opArgs.Length == 2)
                {
                    var types = new Type[] { parameters[0].ParameterType, parameters[1].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,>).MakeGenericType(types),
                        new object[] { opArgs[0], opArgs[1], fn }
                        );
                }
                else if (!method.IsStatic && opArgs.Length == 1)
                {
                    var types = new Type[] { opInst.ReturnType, parameters[0].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,>).MakeGenericType(types),
                        new object[] { opInst, opArgs[0], fn }
                        );
                }
                else if (method.IsStatic && opArgs.Length == 3)
                {
                    var types = new Type[] { parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,,>).MakeGenericType(types),
                        new object[] { opArgs[0], opArgs[1], opArgs[2], fn }
                        );
                }
                else if (!method.IsStatic && opArgs.Length == 2)
                {
                    var types = new Type[] { opInst.ReturnType, parameters[0].ParameterType, parameters[1].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,,>).MakeGenericType(types),
                        new object[] { opInst, opArgs[0], opArgs[1], fn }
                        );
                }
                else if (method.IsStatic && opArgs.Length == 4)
                {
                    var types = new Type[] { parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, parameters[3].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,,,>).MakeGenericType(types),
                        new object[] { opArgs[0], opArgs[1], opArgs[2], opArgs[3], fn }
                        );
                }
                else if (!method.IsStatic && opArgs.Length == 3)
                {
                    var types = new Type[] { opInst.ReturnType, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, method.ReturnType };
                    var fn = Delegate.CreateDelegate(typeof(Func<,,,,>).MakeGenericType(types), null, method);
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FuncEvaluator<,,,,>).MakeGenericType(types),
                        new object[] { opInst, opArgs[0], opArgs[1], opArgs[2], fn }
                        );
                }
                else
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(MethodCallEvaluator<>).MakeGenericType(method.ReturnType),
                        new object[] { method, opInst, opArgs }
                        );
                }
            }

            public Evaluator MemberAccess(MemberExpression m)
            {
                var operand = m.Expression != null ? Build(m.Expression) : null;
                FieldInfo field = m.Member as FieldInfo;
                if (field != null)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(FieldAccessEvaluator<>).MakeGenericType(field.FieldType), 
                        new object[] { operand, field }
                        );
                }
                PropertyInfo property = m.Member as PropertyInfo;
                if (property != null)
                {
                    Type opType = operand != null ? operand.ReturnType : m.Member.DeclaringType;
                    return (Evaluator)Activator.CreateInstance(
                        typeof(PropertyAccessEvaluator<,>).MakeGenericType(opType, property.PropertyType), 
                        new object[] { operand, property }
                        );
                }
                throw new NotSupportedException();
            }

            public Evaluator Conditional(ConditionalExpression c)
            {
                var opTest = Build(c.Test);
                var opIfTrue = Build(c.IfTrue);
                var opIfFalse = Build(c.IfFalse);
                return (Evaluator)Activator.CreateInstance(
                    typeof(ConditionalEvaluator<>).MakeGenericType(c.Type), 
                    new object[] { opTest, opIfTrue, opIfFalse }
                    );
            }

            public Evaluator TypeIs(TypeBinaryExpression t)
            {
                var thing = Build(t.Expression);
                return new TypeIsEvaluator(thing, t.TypeOperand);
            }

            public Evaluator New(NewExpression n)
            {
                var opArgs = n.Arguments.Select(a => Build(a)).ToArray();
                if (opArgs.Length == 0)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluator<>).MakeGenericType(n.Type),
                        new object[] { n.Constructor }
                        );
                }
                else if (opArgs.Length == 1)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluator<,>).MakeGenericType(n.Type, n.Arguments[0].Type),
                        new object[] { n.Constructor, opArgs[0] }
                        );
                }
                else if (opArgs.Length == 2)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluator<,,>).MakeGenericType(n.Type, n.Arguments[0].Type, n.Arguments[1].Type),
                        new object[] { n.Constructor, opArgs[0], opArgs[1] }
                        );
                }
                else if (opArgs.Length == 3)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluator<,,,>).MakeGenericType(n.Type, n.Arguments[0].Type, n.Arguments[1].Type, n.Arguments[2].Type),
                        new object[] { n.Constructor, opArgs[0], opArgs[1], opArgs[2] }
                        );
                }
                else if (opArgs.Length == 4)
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluator<,,,,>).MakeGenericType(n.Type, n.Arguments[0].Type, n.Arguments[1].Type, n.Arguments[2].Type, n.Arguments[3].Type),
                        new object[] { n.Constructor, opArgs[0], opArgs[1], opArgs[2], opArgs[3] }
                        );
                }
                else
                {
                    return (Evaluator)Activator.CreateInstance(
                        typeof(NewEvaluatorN<>).MakeGenericType(n.Type), 
                        new object[] { n.Constructor, opArgs }
                        );
                }
            }

            public Evaluator NewArrayInit(NewArrayExpression n)
            {
                Type elementType = n.Type.GetElementType();
                var initializers = n.Expressions.Select(e => Build(elementType, e)).ToArray();
                return (Evaluator)Activator.CreateInstance(
                    typeof(NewArrayInitEvaluator<>).MakeGenericType(elementType), 
                    new object[] { initializers }
                    );
            }

            public Evaluator NewArrayBounds(NewArrayExpression n)
            {
                var bounds = n.Expressions.Select(e => Build(e)).ToArray();
                return (Evaluator)Activator.CreateInstance(
                    typeof(NewArrayBoundsEvaluator<>).MakeGenericType(n.Type.GetElementType()), 
                    new object[] { bounds }
                    );
            }

            public Evaluator Lambda(LambdaExpression lambda)
            {
                var evaluator = EvaluatorBuilder.Build(this, lambda.Parameters, lambda.Body); 
                return (Evaluator)Activator.CreateInstance(
                    typeof(LambdaEvaluator<>).MakeGenericType(lambda.Type), 
                    new object[] { lambda.Parameters.Count, evaluator }
                    );
            }

            public Evaluator Invoke(InvocationExpression inv)
            {
                var lambda = inv.Expression as LambdaExpression;
                if (lambda != null)
                {
                    // assume parameters from nested lambda area already in scope (see VariableFinder above)
                    Evaluator ev = this.Build(lambda.Body);

                    // make nested let expressions...
                    for (int i = lambda.Parameters.Count - 1; i >= 0; i--)
                    {
                        var parameter = lambda.Parameters[i];
                        int index = this.FindParameterIndex(parameter);
                        var evValue = this.Build(inv.Arguments[i]);
                        ev = (Evaluator)Activator.CreateInstance(
                            typeof(LetEvaluator<,>).MakeGenericType(parameter.Type, lambda.Body.Type),
                            new object[] { index, evValue, ev }
                            );
                    }

                    return ev;
                }
                else 
                {
                    var opFunction = new EvaluatorBuilder(this, null).Build(inv.Expression);
                    var opArgs = inv.Arguments.Select(a => Build(a)).ToArray();
                    return (Evaluator)Activator.CreateInstance(
                        typeof(InvokeEvaluator<>).MakeGenericType(inv.Type), 
                        new object[] { opFunction, opArgs }
                        );
                }
            }

            private Evaluator Quote(UnaryExpression u)
            {
                var external = ExternalReferenceGatherer.Gather(this, u.Operand).ToDictionary(p => this.FindParameterIndex(p));
                return (Evaluator)Activator.CreateInstance(
                    typeof(QuoteEvaluator<>).MakeGenericType(u.Type),
                    new object[] { u.Operand, external }
                    );
            }

            class ExternalReferenceGatherer : ExpressionVisitor
            {
                EvaluatorBuilder builder;
                HashSet<ParameterExpression> external = new HashSet<ParameterExpression>();

                private ExternalReferenceGatherer(EvaluatorBuilder builder)
                {
                    this.builder = builder;
                }

                static internal IEnumerable<ParameterExpression> Gather(EvaluatorBuilder builder, Expression expression)
                {
                    var visitor = new ExternalReferenceGatherer(builder);
                    visitor.Visit(expression);
                    return visitor.external;
                }

                protected override Expression VisitParameter(ParameterExpression p)
                {
                    if (!this.builder.parameters.Contains(p))
                    {
                        this.external.Add(p);
                    }
                    return base.VisitParameter(p);
                }
            }


            private Evaluator MemberInit(MemberInitExpression mini)
            {
                var evNew = Build(mini.NewExpression);
                var initializers = mini.Bindings.Select(b => MemberBinding(mini.Type, b)).ToArray();
                return (Evaluator)Activator.CreateInstance(
                    typeof(InitializerEvaluator<>).MakeGenericType(mini.Type), 
                    new object[] { evNew, initializers }
                    );
            }

            private Evaluator ListInit(ListInitExpression lini)
            {
                var evNew = Build(lini.NewExpression);
                var initializers = lini.Initializers.Select(b => Element(lini.Type, b)).ToArray();
                return (Evaluator)Activator.CreateInstance(
                    typeof(InitializerEvaluator<>).MakeGenericType(lini.Type), 
                    new object[] { evNew, initializers }
                    );
            }

            private Initializer MemberBinding(Type type, MemberBinding mb)
            {
                switch (mb.BindingType)
                {
                    case MemberBindingType.Assignment:
                        return MemberAssignment(type, (MemberAssignment)mb);
                    case MemberBindingType.MemberBinding:
                        return MemberMemberBinding(type, (MemberMemberBinding)mb);
                    case MemberBindingType.ListBinding:
                        return MemberListBinding(type, (MemberListBinding)mb);
                    default:
                        throw new NotImplementedException();
                }
            }

            private Initializer MemberAssignment(Type type, MemberAssignment ma)
            {
                Evaluator evaluator = Build(ma.Expression);
                if (ma.Member is FieldInfo)
                {
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypeFieldAssignmentInitializer<>).MakeGenericType(type),
                            new object[] { ma.Member, evaluator }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(FieldAssignmentInitializer<>).MakeGenericType(type),
                            new object[] { ma.Member, evaluator }
                            );
                    }
                }
                else
                {
                    var property = (PropertyInfo)ma.Member;
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypePropertyAssignmentInitializer<,>).MakeGenericType(type, property.PropertyType),
                            new object[] { property, evaluator }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(PropertyAssignmentInitializer<,>).MakeGenericType(type, property.PropertyType),
                            new object[] { property, evaluator }
                            );
                    }
                }
            }

            private Initializer MemberMemberBinding(Type type, MemberMemberBinding mb)
            {
                FieldInfo fi = mb.Member as FieldInfo;
                if (fi != null)
                {
                    var initializers = mb.Bindings.Select(b => MemberBinding(fi.FieldType, b)).ToArray();
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypeFieldMemberInitializer<,>).MakeGenericType(type, fi.FieldType),
                            new object[] { fi, initializers }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(FieldMemberInitializer<,>).MakeGenericType(type, fi.FieldType), 
                            new object[] { fi, initializers }
                            );
                    }
                }
                PropertyInfo pi = mb.Member as PropertyInfo;
                if (pi != null)
                {
                    var initializers = mb.Bindings.Select(b => MemberBinding(pi.PropertyType, b)).ToArray();
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypePropertyMemberInitializer<,>).MakeGenericType(type, pi.PropertyType),
                            new object[] { pi, initializers }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(PropertyMemberInitializer<,>).MakeGenericType(type, pi.PropertyType), 
                            new object[] { pi, initializers }
                            );
                    }
                }
                throw new InvalidOperationException();
            }

            private Initializer MemberListBinding(Type type, MemberListBinding mb)
            {
                FieldInfo fi = mb.Member as FieldInfo;
                if (fi != null)
                {
                    var initializers = mb.Initializers.Select(e => Element(fi.FieldType, e)).ToArray();
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypeFieldMemberInitializer<,>).MakeGenericType(type, fi.FieldType),
                            new object[] { fi, initializers }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(FieldMemberInitializer<,>).MakeGenericType(type, fi.FieldType), 
                            new object[] { fi, initializers }
                            );
                    }
                }
                PropertyInfo pi = mb.Member as PropertyInfo;
                if (pi != null)
                {
                    var initializers = mb.Initializers.Select(e => Element(pi.PropertyType, e)).ToArray();
                    if (type.IsValueType)
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(ValueTypePropertyMemberInitializer<,>).MakeGenericType(type, pi.PropertyType),
                            new object[] { pi, initializers }
                            );
                    }
                    else
                    {
                        return (Initializer)Activator.CreateInstance(
                            typeof(PropertyMemberInitializer<,>).MakeGenericType(type, pi.PropertyType),
                            new object[] { pi, initializers }
                            );
                    }
                }
                throw new InvalidOperationException();
            }

            private Initializer Element(Type type, ElementInit elem)
            {
                var evArgs = elem.Arguments.Select(a => Build(a)).ToArray();
                if (evArgs.Length == 1)
                {
                    var types = new Type[] { type, evArgs[0].ReturnType };
                    return (Initializer)Activator.CreateInstance(
                        typeof(ElementInitializer<,>).MakeGenericType(types),
                        new object[] { elem.AddMethod, evArgs[0] }
                        );
                }
                else if (evArgs.Length == 2)
                {
                    var types = new Type[] { type, evArgs[0].ReturnType, evArgs[01].ReturnType };
                    return (Initializer)Activator.CreateInstance(
                        typeof(ElementInitializer<,,>).MakeGenericType(types),
                        new object[] { elem.AddMethod, evArgs[0], evArgs[1] }
                        );
                }
                else
                {
                    return (Initializer)Activator.CreateInstance(
                        typeof(ElementInitializer<>).MakeGenericType(type), 
                        new object[] { elem.AddMethod, evArgs }
                        );
                }
            }

            private MethodInfo FindBestMethod(IEnumerable<MethodInfo> methods, Type[] argTypes, Type returnType)
            {
                var meth = methods.FirstOrDefault(m => MethodMatchesExact(m, argTypes, returnType));
                if (meth == null)
                    meth = methods.FirstOrDefault(m => MethodMatches(m, argTypes, returnType));
                return meth;
            }

            private bool MethodMatchesExact(MethodInfo method, Type[] argTypes, Type returnType)
            {
                if (method.ReturnType != returnType)
                    return false;
                var parameters = method.GetParameters();
                if (parameters.Length != argTypes.Length)
                    return false;
                for (int i = 0, n = parameters.Length; i < n; i++)
                {
                    if (parameters[i].ParameterType != argTypes[i])
                        return false;
                }
                return true;
            }

            private bool MethodMatches(MethodInfo method, Type[] argTypes, Type returnType)
            {
                if (returnType != method.ReturnType && !method.ReflectedType.IsSubclassOf(returnType))
                    return false;
                var parameters = method.GetParameters();
                if (parameters.Length != argTypes.Length)
                    return false;
                for (int i = 0, n = parameters.Length; i < n; i++)
                {
                    if (parameters[i].ParameterType != argTypes[i] && !argTypes[i].IsSubclassOf(parameters[i].ParameterType))
                        return false;
                }
                return true;
            }

            private static Type GetNonNullType(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    return type.GetGenericArguments()[0];
                }
                return type;
            }

            private static bool IsNullable(Type type)
            {
                return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
            }
        }

        public abstract class Evaluator
        {
            public abstract object EvalBoxed(EvaluatorState state);
            public abstract Address EvalAddressBoxed(EvaluatorState state);
            public abstract Type ReturnType { get; }
        }

        public abstract class Evaluator<T> : Evaluator
        {
            public abstract T Eval(EvaluatorState state);

            public override object EvalBoxed(EvaluatorState state)
            {
                return this.Eval(state);
            }

            public virtual Address<T> EvalAddress(EvaluatorState state)
            {
                return new ValueAddress<T>(this.Eval(state));
            }

            public override Address EvalAddressBoxed(EvaluatorState state)
            {
                return this.EvalAddress(state);
            }

            public override Type ReturnType
            {
                get { return typeof(T); }
            }
        }

        public abstract class Address
        {
            public abstract object GetBoxedValue(EvaluatorState state);
            public abstract void SetBoxedValue(EvaluatorState state, object value);
        }

        public abstract class Address<T> : Address
        {
            public abstract T GetValue(EvaluatorState state);
            public abstract void SetValue(EvaluatorState state, T value);

            public override object GetBoxedValue(EvaluatorState state)
            {
                return this.GetValue(state);
            }

            public override void SetBoxedValue(EvaluatorState state, object value)
            {
                this.SetValue(state, (T)value);
            }
        }

        public class ValueAddress<T> : Address<T>
        {
            T value;

            public ValueAddress(T value)
            {
                this.value = value;
            }

            public override T GetValue(EvaluatorState state)
            {
                return this.value;
            }

            public override void SetValue(EvaluatorState state, T value)
            {
                this.value = value;
            }
        }

        public class UnaryEvaluator<S, T> : Evaluator<T>
        {
            Evaluator<S> operand;
            Func<S, T> op;

            public UnaryEvaluator(Evaluator<S> operand, Func<S, T> op)
            {
                this.operand = operand;
                this.op = op;
            }

            public override T Eval(EvaluatorState state)
            {
                return op(operand.Eval(state));
            }
        }

        public class LiftToNullUnaryEvaluator<S, T> : Evaluator<T?>
            where S : struct
            where T : struct
        {
            Evaluator<S?> operand;
            Func<S, T> op;

            public LiftToNullUnaryEvaluator(Evaluator<S?> operand, Func<S, T> op)
            {
                this.operand = operand;
                this.op = op;
            }

            public override T? Eval(EvaluatorState state)
            {
                var val = operand.Eval(state);
                if (val == null)
                    return null;
                return op(val.GetValueOrDefault());
            }
        }

        public class LiftToFalseUnaryEvaluator<S> : Evaluator<bool> where S : struct
        {
            Evaluator<S?> operand;
            Func<S, bool> op;

            public LiftToFalseUnaryEvaluator(Evaluator<S?> operand, Func<S, bool> op)
            {
                this.operand = operand;
                this.op = op;
            }

            public override bool Eval(EvaluatorState state)
            {
                var val = operand.Eval(state);
                if (val == null)
                    return false;
                return op(val.GetValueOrDefault());
            }
        }

        public class BinaryEvaluator<S, T> : Evaluator<T>
        {
            Evaluator<S> left;
            Evaluator<S> right;
            Func<S, S, T> op;

            public BinaryEvaluator(Evaluator<S> left, Evaluator<S> right, Func<S, S, T> op)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public override T Eval(EvaluatorState state)
            {
                return op(left.Eval(state), right.Eval(state));
            }
        }

        public class LiftToNullBinaryEvaluator<S, T> : Evaluator<T?>
            where T : struct
            where S : struct
        {
            Evaluator<S?> left;
            Evaluator<S?> right;
            Func<S, S, T> op;

            public LiftToNullBinaryEvaluator(Evaluator<S?> left, Evaluator<S?> right, Func<S, S, T> op)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public override T? Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                var rval = right.Eval(state);
                if (lval == null || rval == null)
                    return null;
                return op(lval.GetValueOrDefault(), rval.GetValueOrDefault());
            }
        }

        public class LiftToFalseBinaryEvaluator<S> : Evaluator<bool> where S : struct
        {
            Evaluator<S?> left;
            Evaluator<S?> right;
            Func<S, S, bool> op;

            public LiftToFalseBinaryEvaluator(Evaluator<S?> left, Evaluator<S?> right, Func<S, S, bool> op)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public override bool Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                var rval = right.Eval(state);
                if (lval == null || rval == null)
                    return false;
                return op(lval.GetValueOrDefault(), rval.GetValueOrDefault());
            }
        }

        public class LiftToEqualBinaryEvaluator<S> : Evaluator<bool> where S : struct
        {
            Evaluator<S?> left;
            Evaluator<S?> right;
            Func<S, S, bool> op;

            public LiftToEqualBinaryEvaluator(Evaluator<S?> left, Evaluator<S?> right, Func<S, S, bool> op)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public override bool Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                var rval = right.Eval(state);
                if (lval == null || rval == null)
                {
                    return (lval == null && rval == null);
                }
                return op(lval.GetValueOrDefault(), rval.GetValueOrDefault());
            }
        }

        public class LiftToNotEqualBinaryEvaluator<S> : Evaluator<bool> where S : struct
        {
            Evaluator<S?> left;
            Evaluator<S?> right;
            Func<S, S, bool> op;

            public LiftToNotEqualBinaryEvaluator(Evaluator<S?> left, Evaluator<S?> right, Func<S, S, bool> op)
            {
                this.left = left;
                this.right = right;
                this.op = op;
            }

            public override bool Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                var rval = right.Eval(state);
                if (lval == null || rval == null)
                {
                    return !(lval == null && rval == null);
                }
                return op(lval.GetValueOrDefault(), rval.GetValueOrDefault());
            }
        }

        public class AndAlsoEvaluator : Evaluator<bool>
        {
            Evaluator<bool> left;
            Evaluator<bool> right;

            public AndAlsoEvaluator(Evaluator<bool> left, Evaluator<bool> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool Eval(EvaluatorState state)
            {
                return left.Eval(state) && right.Eval(state);
            }
        }

        public class LiftToNullAndAlsoEvaluator : Evaluator<bool?>
        {
            Evaluator<bool?> left;
            Evaluator<bool?> right;

            public LiftToNullAndAlsoEvaluator(Evaluator<bool?> left, Evaluator<bool?> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool? Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                if (lval == null) return null;
                if (!lval.GetValueOrDefault()) return false;
                var rval = right.Eval(state);
                if (rval == null) return null;
                return rval.GetValueOrDefault();
            }
        }

        public class LiftToFalseAndAlsoEvaluator : Evaluator<bool>
        {
            Evaluator<bool?> left;
            Evaluator<bool?> right;

            public LiftToFalseAndAlsoEvaluator(Evaluator<bool?> left, Evaluator<bool?> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                if (lval == null) return false;
                var rval = right.Eval(state);
                if (rval == null) return false;
                return lval.GetValueOrDefault() && rval.GetValueOrDefault();
            }
        }

        public class OrElseEvaluator : Evaluator<bool>
        {
            Evaluator<bool> left;
            Evaluator<bool> right;

            public OrElseEvaluator(Evaluator<bool> left, Evaluator<bool> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool Eval(EvaluatorState state)
            {
                return left.Eval(state) || right.Eval(state);
            }
        }

        public class LiftToNullOrElseEvaluator : Evaluator<bool?>
        {
            Evaluator<bool?> left;
            Evaluator<bool?> right;

            public LiftToNullOrElseEvaluator(Evaluator<bool?> left, Evaluator<bool?> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool? Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                if (lval == null) return null;
                if (lval.GetValueOrDefault()) return true;
                var rval = right.Eval(state);
                if (rval == null) return null;
                return rval.GetValueOrDefault();
            }
        }

        public class LiftToFalseOrElseEvaluator : Evaluator<bool>
        {
            Evaluator<bool?> left;
            Evaluator<bool?> right;

            public LiftToFalseOrElseEvaluator(Evaluator<bool?> left, Evaluator<bool?> right)
            {
                this.left = left;
                this.right = right;
            }

            public override bool Eval(EvaluatorState state)
            {
                var lval = left.Eval(state);
                if (lval == null) return false;
                if (lval.GetValueOrDefault()) return true;
                var rval = right.Eval(state);
                if (rval == null) return false;
                return rval.GetValueOrDefault();
            }
        }

        public class Convert<S, T> : Evaluator<T>
        {
            Evaluator<S> operand;
            Func<S, T> fnConvert;

            public Convert(Evaluator<S> operand)
                : this(operand, null)
            {
            }

            public Convert(Evaluator<S> operand, Func<S, T> fnConvert)
            {
                this.operand = operand;
                this.fnConvert = fnConvert;
            }

            public override T Eval(EvaluatorState state)
            {
                var value = this.operand.Eval(state);
                if (this.fnConvert != null)
                {
                    return fnConvert(value);
                }
                else
                {
                    return (T)(object)value;
                }
            }
        }

        public class ConvertNtoNN<T> : Evaluator<T> where T : struct
        {
            Evaluator<T?> operand;

            public ConvertNtoNN(Evaluator<T?> operand)
            {
                this.operand = operand;
            }

            public override T Eval(EvaluatorState state)
            {
                return (T)this.operand.Eval(state);
            }
        }

        public class ConvertNNtoN<T> : Evaluator<T?> where T : struct
        {
            Evaluator<T> operand;

            public ConvertNNtoN(Evaluator<T> operand)
            {
                this.operand = operand;
            }

            public override T? Eval(EvaluatorState state)
            {
                return this.operand.Eval(state);
            }
        }

        public class ConvertNtoNN<S, T> : Evaluator<T>
            where S : struct
            where T : struct
        {
            Evaluator<S?> operand;
            Func<S, T> fnConvert;

            public ConvertNtoNN(Evaluator<S?> operand, Func<S, T> fnConvert)
            {
                this.operand = operand;
                this.fnConvert = fnConvert;
            }

            public override T Eval(EvaluatorState state)
            {
                return this.fnConvert((S)this.operand.Eval(state));
            }
        }

        public class ConvertNNtoN<S, T> : Evaluator<T?>
            where S : struct
            where T : struct
        {
            Evaluator<S> operand;
            Func<S, T> fnConvert;

            public ConvertNNtoN(Evaluator<S> operand, Func<S, T> fnConvert)
            {
                this.operand = operand;
                this.fnConvert = fnConvert;
            }

            public override T? Eval(EvaluatorState state)
            {
                return this.fnConvert(this.operand.Eval(state));
            }
        }

        public class ConvertNtoN<S, T> : Evaluator<T?>
            where S : struct
            where T : struct
        {
            Evaluator<S?> operand;
            Func<S, T> fnConvert;

            public ConvertNtoN(Evaluator<S?> operand, Func<S, T> fnConvert)
            {
                this.operand = operand;
                this.fnConvert = fnConvert;
            }

            public override T? Eval(EvaluatorState state)
            {
                var value = this.operand.Eval(state);
                if (value == null)
                    return null;
                return this.fnConvert(value.GetValueOrDefault());
            }
        }

        public class CoalesceREvaluator<L, R> : Evaluator<R>
        {
            Evaluator<L> opLeft;
            Evaluator<R> opRight;
            Func<L, R> fnConversion;

            public CoalesceREvaluator(Evaluator<L> opLeft, Evaluator<R> opRight, Func<L, R> fnConversion)
            {
                this.opLeft = opLeft;
                this.opRight = opRight;
                this.fnConversion = fnConversion;
            }

            public override R Eval(EvaluatorState state)
            {
                var lval = opLeft.Eval(state);
                if (lval != null)
                    return this.fnConversion(lval);
                return opRight.Eval(state);
            }
        }

        public class CoalesceLEvaluator<L, R> : Evaluator<L> where R : struct
        {
            Evaluator<L> opLeft;
            Evaluator<R> opRight;
            Func<R, L> fnConversion;

            public CoalesceLEvaluator(Evaluator<L> opLeft, Evaluator<R> opRight, Func<R, L> fnConversion)
            {
                this.opLeft = opLeft;
                this.opRight = opRight;
                this.fnConversion = fnConversion;
            }

            public override L Eval(EvaluatorState state)
            {
                var lval = opLeft.Eval(state);
                if (lval != null)
                    return lval;
                return this.fnConversion(opRight.Eval(state));
            }
        }

        public class ConstantEvaluator<T> : Evaluator<T>
        {
            T value;

            public ConstantEvaluator(T value)
            {
                this.value = value;
            }

            public override T Eval(EvaluatorState state)
            {
                return value;
            }
        }

        public class ParameterEvaluator<T> : Evaluator<T>
        {
            int index;

            public ParameterEvaluator(int index)
            {
                this.index = index;
            }

            public override T Eval(EvaluatorState state)
            {
                return state.GetValue<T>(this.index);
            }

            public override Address<T> EvalAddress(EvaluatorState state)
            {
                return new ParameterAddress(this.index);
            }

            class ParameterAddress : Address<T>
            {
                int index;

                public ParameterAddress(int index)
                {
                    this.index = index;
                }

                public override T GetValue(EvaluatorState state)
                {
                    return state.GetValue<T>(this.index);
                }

                public override void SetValue(EvaluatorState state, T value)
                {
                    state.SetValue<T>(this.index, value);
                }
            }
        }

        public class MethodCallEvaluator<T> : Evaluator<T>
        {
            MethodInfo method;
            Evaluator opInst;
            Evaluator[] opArgs;

            public MethodCallEvaluator(MethodInfo method, Evaluator opInst, Evaluator[] opArgs)
            {
                this.method = method;
                this.opInst = opInst;
                this.opArgs = opArgs;
            }

            public override T Eval(EvaluatorState state)
            {
                try
                {
                    object result;
                    if (opInst != null && opInst.ReturnType.IsValueType)
                    {
                        Address addrInstr = opInst.EvalAddressBoxed(state);
                        object instance = addrInstr.GetBoxedValue(state);
                        object[] args = opArgs.Select(a => a.EvalBoxed(state)).ToArray();
                        result = method.Invoke(instance, args);
                        addrInstr.SetBoxedValue(state, instance);
                    }
                    else
                    {
                        object instance = opInst != null ? opInst.EvalBoxed(state) : null;
                        object[] args = opArgs.Select(a => a.EvalBoxed(state)).ToArray();
                        result = method.Invoke(instance, args);
                    }
                    return (T)result;
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class FuncEvaluator<R> : Evaluator<R>
        {
            Func<R> method;

            public FuncEvaluator(Func<R> method)
            {
                this.method = method;
            }

            public override R Eval(EvaluatorState state)
            {
                return this.method();
            }
        }

        public class FuncEvaluator<A, R> : Evaluator<R>
        {
            Evaluator<A> arg;
            Func<A, R> method;

            public FuncEvaluator(Evaluator<A> arg, Func<A, R> method)
            {
                this.arg = arg;
                this.method = method;
            }

            public override R Eval(EvaluatorState state)
            {
                A a = this.arg.Eval(state);
                return this.method(a);
            }
        }

        public class FuncEvaluator<A1, A2, R> : Evaluator<R>
        {
            Evaluator<A1> arg1;
            Evaluator<A2> arg2;
            Func<A1, A2, R> method;

            public FuncEvaluator(Evaluator<A1> arg1, Evaluator<A2> arg2, Func<A1, A2, R> method)
            {
                this.arg1 = arg1;
                this.arg2 = arg2;
                this.method = method;
            }

            public override R Eval(EvaluatorState state)
            {
                return this.method(arg1.Eval(state), arg2.Eval(state));
            }
        }

        public class FuncEvaluator<A1, A2, A3, R> : Evaluator<R>
        {
            Evaluator<A1> arg1;
            Evaluator<A2> arg2;
            Evaluator<A3> arg3;
            Func<A1, A2, A3, R> method;

            public FuncEvaluator(Evaluator<A1> arg1, Evaluator<A2> arg2, Evaluator<A3> arg3, Func<A1, A2, A3, R> method)
            {
                this.arg1 = arg1;
                this.arg2 = arg2;
                this.arg3 = arg3;
                this.method = method;
            }

            public override R Eval(EvaluatorState state)
            {
                return this.method(arg1.Eval(state), arg2.Eval(state), arg3.Eval(state));
            }
        }

        public class FuncEvaluator<A1, A2, A3, A4, R> : Evaluator<R>
        {
            Evaluator<A1> arg1;
            Evaluator<A2> arg2;
            Evaluator<A3> arg3;
            Evaluator<A4> arg4;
            Func<A1, A2, A3, A4, R> method;

            public FuncEvaluator(Evaluator<A1> arg1, Evaluator<A2> arg2, Evaluator<A3> arg3, Evaluator<A4> arg4, Func<A1, A2, A3, A4, R> method)
            {
                this.arg1 = arg1;
                this.arg2 = arg2;
                this.arg3 = arg3;
                this.arg4 = arg4;
                this.method = method;
            }

            public override R Eval(EvaluatorState state)
            {
                return this.method(arg1.Eval(state), arg2.Eval(state), arg3.Eval(state), arg4.Eval(state));
            }
        }

        public class MethodCallWithRefArgsEvaluator<T> : Evaluator<T>
        {
            MethodInfo method;
            ParameterInfo[] parameters;
            Evaluator opInst;
            Evaluator[] opArgs;

            public MethodCallWithRefArgsEvaluator(MethodInfo method, Evaluator opInst, Evaluator[] opArgs)
            {
                this.method = method;
                this.parameters = method.GetParameters();
                this.opInst = opInst;
                this.opArgs = opArgs;
            }

            public override T Eval(EvaluatorState state)
            {
                try
                {
                    Address addrInstr = null;
                    object instance;
                    Address[] addrs;
                    object[] args;
                    object result;

                    if (opInst != null && opInst.ReturnType.IsValueType)
                    {
                        addrInstr = opInst.EvalAddressBoxed(state);
                        instance = addrInstr.GetBoxedValue(state);
                    }
                    else
                    {
                        instance = opInst != null ? opInst.EvalBoxed(state) : null;
                    }

                    addrs = opArgs.Select(a => a.EvalAddressBoxed(state)).ToArray();
                    args = addrs.Select(a => a.GetBoxedValue(state)).ToArray();
                    result = method.Invoke(instance, args);

                    for (int i = 0, n = args.Length; i < n; i++)
                    {
                        if (this.parameters[i].ParameterType.IsByRef)
                        {
                            addrs[i].SetBoxedValue(state, args[i]);
                        }
                    }

                    if (addrInstr != null)
                    {
                        addrInstr.SetBoxedValue(state, instance);
                    }

                    return (T)result;
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class LiftToNullMethodCallEvaluator<T> : Evaluator<T?> where T : struct
        {
            MethodInfo method;
            Evaluator opInst;
            Evaluator[] opArgs;

            public LiftToNullMethodCallEvaluator(MethodInfo method, Evaluator opInst, Evaluator[] opArgs)
            {
                this.method = method;
                this.opInst = opInst;
                this.opArgs = opArgs;
            }

            public override T? Eval(EvaluatorState state)
            {
                object instance = opInst != null ? opInst.EvalBoxed(state) : null;
                object[] args = opArgs.Select(a => a.EvalBoxed(state)).ToArray();
                if (instance == null || args.Any(a => a == null))
                    return null;
                try
                {
                    return (T?)method.Invoke(instance, args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class LiftToFalseMethodCallEvaluator : Evaluator<bool>
        {
            MethodInfo method;
            Evaluator opInst;
            Evaluator[] opArgs;

            public LiftToFalseMethodCallEvaluator(MethodInfo method, Evaluator opInst, Evaluator[] opArgs)
            {
                this.method = method;
                this.opInst = opInst;
                this.opArgs = opArgs;
            }

            public override bool Eval(EvaluatorState state)
            {
                object instance = opInst != null ? opInst.EvalBoxed(state) : null;
                object[] args = opArgs.Select(a => a.EvalBoxed(state)).ToArray();
                if (instance == null || args.Any(a => a == null))
                    return false;
                try
                {
                    return (bool)method.Invoke(instance, args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class FieldAccessEvaluator<T> : Evaluator<T>
        {
            Evaluator operand;
            FieldInfo field;

            public FieldAccessEvaluator(Evaluator operand, FieldInfo field)
            {
                this.operand = operand;
                this.field = field;
            }

            public override T Eval(EvaluatorState state)
            {
                var instance = this.operand != null ? this.operand.EvalBoxed(state) : null;
                return (T)field.GetValue(instance);
            }

            public override Address<T> EvalAddress(EvaluatorState state)
            {
                Address addr = this.operand != null ? this.operand.EvalAddressBoxed(state) : null;
                return new FieldAddress(addr, this.field);
            }

            class FieldAddress : Address<T>
            {
                Address instance;
                FieldInfo field;

                public FieldAddress(Address instance, FieldInfo field)
                {
                    this.instance = instance;
                    this.field = field;
                }

                public override T GetValue(EvaluatorState state)
                {
                    object boxedInst = this.instance != null ? this.instance.GetBoxedValue(state) : null;
                    return (T)field.GetValue(boxedInst);
                }

                public override void SetValue(EvaluatorState state, T value)
                {
                    object boxedInst = this.instance != null ? this.instance.GetBoxedValue(state) : null;
                    field.SetValue(boxedInst, value);
                    if (this.instance != null)
                    {
                        this.instance.SetBoxedValue(state, boxedInst);
                    }
                }
            }
        }

        public class PropertyAccessEvaluator<T, V> : Evaluator<V>
        {
            Evaluator<T> operand;
            Func<T, V> fnGetter;

            public PropertyAccessEvaluator(Evaluator<T> operand, PropertyInfo property)
            {
                this.operand = operand;
                this.fnGetter = (Func<T,V>)Delegate.CreateDelegate(typeof(Func<T, V>), property.GetGetMethod(true));
            }

            public override V Eval(EvaluatorState state)
            {
                T item = this.operand != null ? this.operand.Eval(state) : default(T);
                return this.fnGetter(item);
            }
        }

        public class ConditionalEvaluator<T> : Evaluator<T>
        {
            Evaluator<bool> test;
            Evaluator<T> ifTrue;
            Evaluator<T> ifFalse;

            public ConditionalEvaluator(Evaluator<bool> test, Evaluator<T> ifTrue, Evaluator<T> ifFalse)
            {
                this.test = test;
                this.ifTrue = ifTrue;
                this.ifFalse = ifFalse;
            }

            public override T Eval(EvaluatorState state)
            {
                return (test.Eval(state)) ? ifTrue.Eval(state) : ifFalse.Eval(state);
            }
        }

        public class TypeIsEvaluator : Evaluator<bool>
        {
            Evaluator thing;
            Type type;

            public TypeIsEvaluator(Evaluator thing, Type type)
            {
                this.thing = thing;
                this.type = type;
            }

            public override bool Eval(EvaluatorState state)
            {
                var result = thing.EvalBoxed(state);
                if (result == null) return false;
                Type resultType = result.GetType();
                return (resultType == type || resultType.IsSubclassOf(type));
            }
        }

        public class TypeAsEvaluator<S, T> : Evaluator<T>
            where S : class
            where T : class
        {
            Evaluator<S> operand;

            public TypeAsEvaluator(Evaluator<S> operand)
            {
                this.operand = operand;
            }

            public override T Eval(EvaluatorState state)
            {
                return operand.Eval(state) as T;
            }
        }

        private static Func<Type, object, RuntimeMethodHandle, Delegate> fnCreateDelegate;
        private static Delegate CreateDelegate(Type delegateType, ConstructorInfo constructor)
        {
            if (fnCreateDelegate == null)
            {
                MethodInfo miCreateDelegate =
                    typeof(Delegate).GetMethod(
                        "CreateDelegate", BindingFlags.Static | BindingFlags.NonPublic, null,
                        new Type[] { typeof(Type), typeof(object), typeof(RuntimeMethodHandle) }, null
                        );
                fnCreateDelegate = (Func<Type, object, RuntimeMethodHandle, Delegate>)
                    Delegate.CreateDelegate(typeof(Func<Type, object, RuntimeMethodHandle, Delegate>), miCreateDelegate);
            }
            return fnCreateDelegate(delegateType, null, constructor.MethodHandle);
        }

        public class NewEvaluator<T> : Evaluator<T>
        {
            Action<T> fnConstructor;

            public NewEvaluator(ConstructorInfo constructor)
            {
                if (constructor != null)
                    this.fnConstructor = (Action<T>)CreateDelegate(typeof(Action<T>), constructor);
            }

            public override T Eval(EvaluatorState state)
            {
                T t = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                if (this.fnConstructor != null)
                    this.fnConstructor(t);
                return t;
            }
        }

        public class NewEvaluator<T, A> : Evaluator<T>
        {
            Action<T, A> fnConstructor;
            Evaluator<A> opArg;

            public NewEvaluator(ConstructorInfo constructor, Evaluator<A> opArg)
            {
                this.fnConstructor = (Action<T, A>)CreateDelegate(typeof(Action<T, A>), constructor);
                this.opArg = opArg;
            }

            public override T Eval(EvaluatorState state)
            {
                A a = this.opArg.Eval(state);
                T t = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                this.fnConstructor(t, a);
                return t;
            }
        }

        public class NewEvaluator<T, A1, A2> : Evaluator<T>
        {
            Action<T, A1, A2> fnConstructor;
            Evaluator<A1> opArg1;
            Evaluator<A2> opArg2;

            public NewEvaluator(ConstructorInfo constructor, Evaluator<A1> opArg1, Evaluator<A2> opArg2)
            {
                this.fnConstructor = (Action<T, A1, A2>)CreateDelegate(typeof(Action<T, A1, A2>), constructor);
                this.opArg1 = opArg1;
                this.opArg2 = opArg2;
            }

            public override T Eval(EvaluatorState state)
            {
                A1 a1 = this.opArg1.Eval(state);
                A2 a2 = this.opArg2.Eval(state);
                T t = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                this.fnConstructor(t, a1, a2);
                return t;
            }
        }

        public class NewEvaluator<T, A1, A2, A3> : Evaluator<T>
        {
            Action<T, A1, A2, A3> fnConstructor;
            Evaluator<A1> opArg1;
            Evaluator<A2> opArg2;
            Evaluator<A3> opArg3;

            public NewEvaluator(ConstructorInfo constructor, Evaluator<A1> opArg1, Evaluator<A2> opArg2, Evaluator<A3> opArg3)
            {
                this.fnConstructor = (Action<T, A1, A2, A3>)CreateDelegate(typeof(Action<T, A1, A2, A3>), constructor);
                this.opArg1 = opArg1;
                this.opArg2 = opArg2;
                this.opArg3 = opArg3;
            }

            public override T Eval(EvaluatorState state)
            {
                A1 a1 = this.opArg1.Eval(state);
                A2 a2 = this.opArg2.Eval(state);
                A3 a3 = this.opArg3.Eval(state);
                T t = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                this.fnConstructor(t, a1, a2, a3);
                return t;
            }
        }

        public class NewEvaluator<T, A1, A2, A3, A4> : Evaluator<T>
        {
            delegate void Action(T t, A1 a1, A2 a2, A3 a3, A4 a4);
            Action fnConstructor;
            Evaluator<A1> opArg1;
            Evaluator<A2> opArg2;
            Evaluator<A3> opArg3;
            Evaluator<A4> opArg4;

            public NewEvaluator(ConstructorInfo constructor, Evaluator<A1> opArg1, Evaluator<A2> opArg2, Evaluator<A3> opArg3, Evaluator<A4> opArg4)
            {
                this.fnConstructor = (Action)CreateDelegate(typeof(Action), constructor);
                this.opArg1 = opArg1;
                this.opArg2 = opArg2;
                this.opArg3 = opArg3;
                this.opArg4 = opArg4;
            }

            public override T Eval(EvaluatorState state)
            {
                A1 a1 = this.opArg1.Eval(state);
                A2 a2 = this.opArg2.Eval(state);
                A3 a3 = this.opArg3.Eval(state);
                A4 a4 = this.opArg4.Eval(state);
                T t = (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
                this.fnConstructor(t, a1, a2, a3, a4);
                return t;
            }
        }

        public class NewEvaluatorN<T> : Evaluator<T>
        {
            ConstructorInfo constructor;
            Evaluator[] opArgs;

            public NewEvaluatorN(ConstructorInfo constructor, Evaluator[] opArgs)
            {
                this.constructor = constructor;
                this.opArgs = opArgs;
            }

            public override T Eval(EvaluatorState state)
            {
                var args = opArgs.Select(a => a.EvalBoxed(state)).ToArray();
                try
                {
                    return (T)constructor.Invoke(args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class NewArrayInitEvaluator<T> : Evaluator<T[]>
        {
            Evaluator<T>[] initializers;

            public NewArrayInitEvaluator(Evaluator[] initializers)
            {
                this.initializers = initializers.Select(i => (Evaluator<T>)i).ToArray();
            }

            public override T[] Eval(EvaluatorState state)
            {
                T[] array = new T[initializers.Length];
                for (int i = 0, n = initializers.Length; i < n; i++)
                {
                    array[i] = initializers[i].Eval(state);
                }
                return array;
            }
        }

        public class NewArrayBoundsEvaluator<T> : Evaluator<T[]>
        {
            Evaluator<int>[] bounds;

            public NewArrayBoundsEvaluator(Evaluator[] bounds)
            {
                this.bounds = bounds.Select(b => (Evaluator<int>)b).ToArray();
            }

            public override T[] Eval(EvaluatorState state)
            {
                int[] bounds = this.bounds.Select(b => b.Eval(state)).ToArray();
                return (T[])Array.CreateInstance(typeof(T), bounds);
            }
        }

        public class ArrayIndexEvaluator<T> : Evaluator<T>
        {
            Evaluator<T[]> opArray;
            Evaluator<int> opIndex;

            public ArrayIndexEvaluator(Evaluator<T[]> opArray, Evaluator<int> opIndex)
            {
                this.opArray = opArray;
                this.opIndex = opIndex;
            }

            public override T Eval(EvaluatorState state)
            {
                return this.opArray.Eval(state)[this.opIndex.Eval(state)];
            }

            public override Address<T> EvalAddress(EvaluatorState state)
            {
                return new ArrayElementAddress(this.opArray.Eval(state), this.opIndex.Eval(state));
            }

            class ArrayElementAddress : Address<T>
            {
                T[] array;
                int index;

                public ArrayElementAddress(T[] array, int index)
                {
                    this.array = array;
                    this.index = index;
                }

                public override T GetValue(EvaluatorState state)
                {
                    return this.array[this.index];
                }

                public override void SetValue(EvaluatorState state, T value)
                {
                    this.array[this.index] = value;
                }
            }
        }

        public class ArrayLengthEvaluator<T> : Evaluator<int>
        {
            Evaluator<T[]> opArray;

            public ArrayLengthEvaluator(Evaluator<T[]> opArray)
            {
                this.opArray = opArray;
            }

            public override int Eval(EvaluatorState state)
            {
                return this.opArray.Eval(state).Length;
            }
        }

        public class LambdaEvaluator<T> : Evaluator<T> where T : class
        {
            int nArgs;
            Evaluator evaluator;

            public LambdaEvaluator(int nArgs, Evaluator evaluator)
            {
                this.nArgs = nArgs;
                this.evaluator = evaluator;
            }

            public override T Eval(EvaluatorState state)
            {
                T d = (T)(object)CreateDelegate(typeof(T), state, this.nArgs, this.evaluator);
                return d;
            }
        }

        public class LetEvaluator<V, E> : Evaluator<E>
        {
            int index;
            Evaluator<V> evValue;
            Evaluator<E> evExpression;

            public LetEvaluator(int index, Evaluator<V> evValue, Evaluator<E> evExpression)
            {
                this.index = index;
                this.evValue = evValue;
                this.evExpression = evExpression;
            }

            public override E Eval(EvaluatorState state)
            {
                V value = this.evValue.Eval(state);
                state.SetValue<V>(this.index, value);
                return this.evExpression.Eval(state);
            }
        }

        public class InvokeEvaluator<T> : Evaluator<T>
        {
            Evaluator opFunction;
            Evaluator[] opArgs;

            public InvokeEvaluator(Evaluator opFunction, Evaluator[] opArgs)
            {
                this.opFunction = opFunction;
                this.opArgs = opArgs;
            }

            public override T Eval(EvaluatorState state)
            {
                var function = opFunction.EvalBoxed(state);
                var args = opArgs.Select(a => a.EvalBoxed(state));
                try
                {
                    return (T)((Delegate)function).DynamicInvoke(args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class QuoteEvaluator<T> : Evaluator<T> where T : Expression
        {
            Expression expression;
            Dictionary<ParameterExpression, int> external;

            public QuoteEvaluator(Expression expression, Dictionary<ParameterExpression, int> external)
            {
                this.expression = expression;
                this.external = external;
            }

            public override T Eval(EvaluatorState state)
            {
                if (external.Count > 0)
                {
                    // replace external parameter references with current values
                    return (T)QuoteRewriter.Rewrite(this.external, state, this.expression);
                }
                return (T)this.expression;
            }

            class QuoteRewriter : ExpressionVisitor
            {
                Dictionary<ParameterExpression, int> external;
                EvaluatorState state;

                private QuoteRewriter(Dictionary<ParameterExpression, int> external, EvaluatorState state)
                {
                    this.external = external;
                    this.state = state;
                }

                internal static Expression Rewrite(Dictionary<ParameterExpression, int> external, EvaluatorState state, Expression expression)
                {
                    return new QuoteRewriter(external, state).Visit(expression);
                }

                protected override Expression VisitParameter(ParameterExpression p)
                {
                    int externalIndex;
                    if (this.external.TryGetValue(p, out externalIndex))
                    {
                        return Expression.Constant(this.state.GetBoxedValue(externalIndex), p.Type);
                    }
                    return p;
                }
            }
        }

        public class InitializerEvaluator<T> : Evaluator<T>
        {
            Evaluator<T> opNew;
            Initializer<T>[] initializers;

            public InitializerEvaluator(Evaluator<T> opNew, Initializer[] initializers)
            {
                this.opNew = opNew;
                this.initializers = initializers.Select(i => (Initializer<T>)i).ToArray();
            }

            public override T Eval(EvaluatorState state)
            {
                var instance = opNew.Eval(state);
                for (int i = 0, n = initializers.Length; i < n; i++)
                {
                    initializers[i].Init(state, ref instance);
                }
                return instance;
            }
        }

        public abstract class Initializer
        {
        }

        public abstract class Initializer<T> : Initializer
        {
            public abstract void Init(EvaluatorState state, ref T instance);
        }

        public class FieldAssignmentInitializer<T> : Initializer<T> where T : class
        {
            FieldInfo field;
            Evaluator evaluator;

            public FieldAssignmentInitializer(FieldInfo field, Evaluator evaluator)
            {
                this.field = field;
                this.evaluator = evaluator;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                var value = this.evaluator.EvalBoxed(state);
                field.SetValue(instance, value);
            }
        }

        public class ValueTypeFieldAssignmentInitializer<T> : Initializer<T> where T : struct
        {
            FieldInfo field;
            Evaluator evaluator;

            public ValueTypeFieldAssignmentInitializer(FieldInfo field, Evaluator evaluator)
            {
                this.field = field;
                this.evaluator = evaluator;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                var value = this.evaluator.EvalBoxed(state);
                object boxedInstance = instance;
                field.SetValue(boxedInstance, value);
                instance = (T)boxedInstance;
            }
        }

        public class ValueTypePropertyAssignmentInitializer<T, V> : Initializer<T> where T : struct
        {
            delegate void Assigner(ref T instance, V value);
            Assigner fnSetter;
            Evaluator<V> evaluator;

            public ValueTypePropertyAssignmentInitializer(PropertyInfo property, Evaluator<V> evaluator)
            {
                this.fnSetter = (Assigner)Delegate.CreateDelegate(typeof(Assigner), property.GetSetMethod(true));
                this.evaluator = evaluator;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                V value = this.evaluator.Eval(state);
                fnSetter(ref instance, value);
            }
        }

        public class PropertyAssignmentInitializer<T, V> : Initializer<T> where T : class
        {
            Action<T, V> fnSetter;
            Evaluator<V> evaluator;

            public PropertyAssignmentInitializer(PropertyInfo property, Evaluator<V> evaluator)
            {
                this.fnSetter = (Action<T,V>)Delegate.CreateDelegate(typeof(Action<T,V>), property.GetSetMethod(true));
                this.evaluator = evaluator;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                V value = this.evaluator.Eval(state);
                fnSetter(instance, value);
            }
        }

        public class FieldMemberInitializer<T, V> : Initializer<T> where T : class
        {
            FieldInfo field;
            Initializer<V>[] initializers;

            public FieldMemberInitializer(FieldInfo field, Initializer[] initializers)
            {
                this.field = field;
                this.initializers = initializers.Select(i => (Initializer<V>)i).ToArray();
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                V value = (V)field.GetValue(instance);
                for (int i = 0, n = this.initializers.Length; i < n; i++)
                {
                    this.initializers[i].Init(state, ref value);
                }
                field.SetValue(instance, value);
            }
        }

        public class ValueTypeFieldMemberInitializer<T, V> : Initializer<T> where T : struct
        {
            FieldInfo field;
            Initializer<V>[] initializers;
            bool valueIsValueType;

            public ValueTypeFieldMemberInitializer(FieldInfo field, Initializer[] initializers)
            {
                this.field = field;
                this.initializers = initializers.Select(i => (Initializer<V>)i).ToArray();
                this.valueIsValueType = field.FieldType.IsValueType;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                object boxedInstance = instance;
                V value = (V)field.GetValue(boxedInstance);
                for (int i = 0, n = this.initializers.Length; i < n; i++)
                {
                    this.initializers[i].Init(state, ref value);
                }
                if (this.valueIsValueType)
                    field.SetValue(boxedInstance, value);
                instance = (T)boxedInstance;
            }
        }

        public class PropertyMemberInitializer<T, V> : Initializer<T>
            where T : class
        {
            Func<T, V> fnGetter;
            Initializer<V>[] initializers;

            public PropertyMemberInitializer(PropertyInfo property, Initializer[] initializers)
            {
                this.fnGetter = (Func<T, V>)Delegate.CreateDelegate(typeof(Func<T, V>), property.GetGetMethod(true));
                this.initializers = initializers.Select(i => (Initializer<V>)i).ToArray();
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                V value = this.fnGetter(instance);
                for (int i = 0, n = this.initializers.Length; i < n; i++)
                {
                    this.initializers[i].Init(state, ref value);
                }
            }
        }

        public class ValueTypePropertyMemberInitializer<T, V> : Initializer<T> 
            where T : struct
        {
            delegate V Getter(ref T instance);
            Getter fnGetter;
            Initializer<V>[] initializers;

            public ValueTypePropertyMemberInitializer(PropertyInfo property, Initializer[] initializers)
            {
                this.fnGetter = (Getter)Delegate.CreateDelegate(typeof(Getter), property.GetGetMethod(true));
                this.initializers = initializers.Select(i => (Initializer<V>)i).ToArray();
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                V value = this.fnGetter(ref instance);
                for (int i = 0, n = this.initializers.Length; i < n; i++)
                {
                    this.initializers[i].Init(state, ref value);
                }
            }
        }

        public class ElementInitializer<T> : Initializer<T>
            where T : class
        {
            MethodInfo addMethod;
            Evaluator[] evArgs;

            public ElementInitializer(MethodInfo addMethod, Evaluator[] evArgs)
            {
                this.addMethod = addMethod;
                this.evArgs = evArgs;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                try
                {
                    var args = evArgs.Select(a => a.EvalBoxed(state)).ToArray();
                    addMethod.Invoke(instance, args);
                }
                catch (TargetInvocationException tie)
                {
                    throw tie.InnerException;
                }
            }
        }

        public class ElementInitializer<T, A> : Initializer<T>
            where T : class
        {
            Action<T, A> fnAdder;
            Evaluator<A> evArg;

            public ElementInitializer(MethodInfo addMethod, Evaluator<A> evArg)
            {
                this.fnAdder = (Action<T, A>)Delegate.CreateDelegate(typeof(Action<T, A>), addMethod);
                this.evArg = evArg;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                A arg = this.evArg.Eval(state);
                this.fnAdder(instance, arg);
            }
        }

        public class ElementInitializer<T, A1, A2> : Initializer<T>
            where T : class
        {
            Action<T, A1, A2> fnAdder;
            Evaluator<A1> evArg1;
            Evaluator<A2> evArg2;

            public ElementInitializer(MethodInfo addMethod, Evaluator<A1> evArg1, Evaluator<A2> evArg2)
            {
                this.fnAdder = (Action<T, A1, A2>)Delegate.CreateDelegate(typeof(Action<T, A1, A2>), addMethod);
                this.evArg1 = evArg1;
                this.evArg2 = evArg2;
            }

            public override void Init(EvaluatorState state, ref T instance)
            {
                A1 arg1 = this.evArg1.Eval(state);
                A2 arg2 = this.evArg2.Eval(state);
                this.fnAdder(instance, arg1, arg2);
            }
        }

        public static class Operators
        {
            public static SByte Add(SByte left, SByte right) { return (SByte)(left + right); }
            public static Int16 Add(Int16 left, Int16 right) { return (Int16)(left + right); }
            public static Int32 Add(Int32 left, Int32 right) { return (Int32)(left + right); }
            public static Int64 Add(Int64 left, Int64 right) { return (Int64)(left + right); }
            public static Byte Add(Byte left, Byte right) { return (Byte)(left + right); }
            public static UInt16 Add(UInt16 left, UInt16 right) { return (UInt16)(left + right); }
            public static UInt32 Add(UInt32 left, UInt32 right) { return (UInt32)(left + right); }
            public static UInt64 Add(UInt64 left, UInt64 right) { return (UInt64)(left + right); }
            public static Single Add(Single left, Single right) { return (Single)(left + right); }
            public static Double Add(Double left, Double right) { return (Double)(left + right); }
            public static Decimal Add(Decimal left, Decimal right) { return (Decimal)(left + right); }
            public static SByte AddChecked(SByte left, SByte right) { return checked((SByte)(left + right)); }
            public static Int16 AddChecked(Int16 left, Int16 right) { return checked((Int16)(left + right)); }
            public static Int32 AddChecked(Int32 left, Int32 right) { return checked((Int32)(left + right)); }
            public static Int64 AddChecked(Int64 left, Int64 right) { return checked((Int64)(left + right)); }
            public static Byte AddChecked(Byte left, Byte right) { return checked((Byte)(left + right)); }
            public static UInt16 AddChecked(UInt16 left, UInt16 right) { return checked((UInt16)(left + right)); }
            public static UInt32 AddChecked(UInt32 left, UInt32 right) { return checked((UInt32)(left + right)); }
            public static UInt64 AddChecked(UInt64 left, UInt64 right) { return checked((UInt64)(left + right)); }
            public static Single AddChecked(Single left, Single right) { return checked((Single)(left + right)); }
            public static Double AddChecked(Double left, Double right) { return checked((Double)(left + right)); }
            public static Decimal AddChecked(Decimal left, Decimal right) { return checked((Decimal)(left + right)); }

            public static SByte Subtract(SByte left, SByte right) { return (SByte)(left - right); }
            public static Int16 Subtract(Int16 left, Int16 right) { return (Int16)(left - right); }
            public static Int32 Subtract(Int32 left, Int32 right) { return (Int32)(left - right); }
            public static Int64 Subtract(Int64 left, Int64 right) { return (Int64)(left - right); }
            public static Byte Subtract(Byte left, Byte right) { return (Byte)(left - right); }
            public static UInt16 Subtract(UInt16 left, UInt16 right) { return (UInt16)(left - right); }
            public static UInt32 Subtract(UInt32 left, UInt32 right) { return (UInt32)(left - right); }
            public static UInt64 Subtract(UInt64 left, UInt64 right) { return (UInt64)(left - right); }
            public static Single Subtract(Single left, Single right) { return (Single)(left - right); }
            public static Double Subtract(Double left, Double right) { return (Double)(left - right); }
            public static Decimal Subtract(Decimal left, Decimal right) { return (Decimal)(left - right); }
            public static SByte SubtractChecked(SByte left, SByte right) { return checked((SByte)(left - right)); }
            public static Int16 SubtractChecked(Int16 left, Int16 right) { return checked((Int16)(left - right)); }
            public static Int32 SubtractChecked(Int32 left, Int32 right) { return checked((Int32)(left - right)); }
            public static Int64 SubtractChecked(Int64 left, Int64 right) { return checked((Int64)(left - right)); }
            public static Byte SubtractChecked(Byte left, Byte right) { return checked((Byte)(left - right)); }
            public static UInt16 SubtractChecked(UInt16 left, UInt16 right) { return checked((UInt16)(left - right)); }
            public static UInt32 SubtractChecked(UInt32 left, UInt32 right) { return checked((UInt32)(left - right)); }
            public static UInt64 SubtractChecked(UInt64 left, UInt64 right) { return checked((UInt64)(left - right)); }
            public static Single SubtractChecked(Single left, Single right) { return checked((Single)(left - right)); }
            public static Double SubtractChecked(Double left, Double right) { return checked((Double)(left - right)); }
            public static Decimal SubtractChecked(Decimal left, Decimal right) { return checked((Decimal)(left - right)); }

            public static SByte Multiply(SByte left, SByte right) { return (SByte)(left * right); }
            public static Int16 Multiply(Int16 left, Int16 right) { return (Int16)(left * right); }
            public static Int32 Multiply(Int32 left, Int32 right) { return (Int32)(left * right); }
            public static Int64 Multiply(Int64 left, Int64 right) { return (Int64)(left * right); }
            public static Byte Multiply(Byte left, Byte right) { return (Byte)(left * right); }
            public static UInt16 Multiply(UInt16 left, UInt16 right) { return (UInt16)(left * right); }
            public static UInt32 Multiply(UInt32 left, UInt32 right) { return (UInt32)(left * right); }
            public static UInt64 Multiply(UInt64 left, UInt64 right) { return (UInt64)(left * right); }
            public static Single Multiply(Single left, Single right) { return (Single)(left * right); }
            public static Double Multiply(Double left, Double right) { return (Double)(left * right); }
            public static Decimal Multiply(Decimal left, Decimal right) { return (Decimal)(left * right); }
            public static SByte MultiplyChecked(SByte left, SByte right) { return checked((SByte)(left * right)); }
            public static Int16 MultiplyChecked(Int16 left, Int16 right) { return checked((Int16)(left * right)); }
            public static Int32 MultiplyChecked(Int32 left, Int32 right) { return checked((Int32)(left * right)); }
            public static Int64 MultiplyChecked(Int64 left, Int64 right) { return checked((Int64)(left * right)); }
            public static Byte MultiplyChecked(Byte left, Byte right) { return checked((Byte)(left * right)); }
            public static UInt16 MultiplyChecked(UInt16 left, UInt16 right) { return checked((UInt16)(left * right)); }
            public static UInt32 MultiplyChecked(UInt32 left, UInt32 right) { return checked((UInt32)(left * right)); }
            public static UInt64 MultiplyChecked(UInt64 left, UInt64 right) { return checked((UInt64)(left * right)); }
            public static Single MultiplyChecked(Single left, Single right) { return checked((Single)(left * right)); }
            public static Double MultiplyChecked(Double left, Double right) { return checked((Double)(left * right)); }
            public static Decimal MultiplyChecked(Decimal left, Decimal right) { return checked((Decimal)(left * right)); }

            public static SByte Divide(SByte left, SByte right) { return (SByte)(left / right); }
            public static Int16 Divide(Int16 left, Int16 right) { return (Int16)(left / right); }
            public static Int32 Divide(Int32 left, Int32 right) { return (Int32)(left / right); }
            public static Int64 Divide(Int64 left, Int64 right) { return (Int64)(left / right); }
            public static Byte Divide(Byte left, Byte right) { return (Byte)(left / right); }
            public static UInt16 Divide(UInt16 left, UInt16 right) { return (UInt16)(left / right); }
            public static UInt32 Divide(UInt32 left, UInt32 right) { return (UInt32)(left / right); }
            public static UInt64 Divide(UInt64 left, UInt64 right) { return (UInt64)(left / right); }
            public static Single Divide(Single left, Single right) { return (Single)(left / right); }
            public static Double Divide(Double left, Double right) { return (Double)(left / right); }
            public static Decimal Divide(Decimal left, Decimal right) { return (Decimal)(left / right); }

            public static SByte Modulo(SByte left, SByte right) { return (SByte)(left % right); }
            public static Int16 Modulo(Int16 left, Int16 right) { return (Int16)(left % right); }
            public static Int32 Modulo(Int32 left, Int32 right) { return (Int32)(left % right); }
            public static Int64 Modulo(Int64 left, Int64 right) { return (Int64)(left % right); }
            public static Byte Modulo(Byte left, Byte right) { return (Byte)(left % right); }
            public static UInt16 Modulo(UInt16 left, UInt16 right) { return (UInt16)(left % right); }
            public static UInt32 Modulo(UInt32 left, UInt32 right) { return (UInt32)(left % right); }
            public static UInt64 Modulo(UInt64 left, UInt64 right) { return (UInt64)(left % right); }
            public static Single Modulo(Single left, Single right) { return (Single)(left % right); }
            public static Double Modulo(Double left, Double right) { return (Double)(left % right); }
            public static Decimal Modulo(Decimal left, Decimal right) { return (Decimal)(left % right); }

            public static SByte Power(SByte left, SByte right) { return (SByte)Math.Pow(left, right); }
            public static Int16 Power(Int16 left, Int16 right) { return (Int16)Math.Pow(left, right); }
            public static Int32 Power(Int32 left, Int32 right) { return (Int32)Math.Pow(left, right); }
            public static Int64 Power(Int64 left, Int64 right) { return (Int64)Math.Pow(left, right); }
            public static Byte Power(Byte left, Byte right) { return (Byte)Math.Pow(left, right); }
            public static UInt16 Power(UInt16 left, UInt16 right) { return (UInt16)Math.Pow(left, right); }
            public static UInt32 Power(UInt32 left, UInt32 right) { return (UInt32)Math.Pow(left, right); }
            public static UInt64 Power(UInt64 left, UInt64 right) { return (UInt64)Math.Pow(left, right); }
            public static Single Power(Single left, Single right) { return (Single)Math.Pow(left, right); }
            public static Double Power(Double left, Double right) { return (Double)Math.Pow(left, right); }
            public static Decimal Power(Decimal left, Decimal right) { return (Decimal)Math.Pow((double)left, (double)right); }

            public static SByte LeftShift(SByte left, SByte right) { return (SByte)(left << right); }
            public static Int16 LeftShift(Int16 left, Int16 right) { return (Int16)(left << right); }
            public static Int32 LeftShift(Int32 left, Int32 right) { return (Int32)(left << right); }
            public static Int64 LeftShift(Int64 left, Int64 right) { return (Int64)(left << (int)right); }
            public static Byte LeftShift(Byte left, Byte right) { return (Byte)(left << right); }
            public static UInt16 LeftShift(UInt16 left, UInt16 right) { return (UInt16)(left << right); }
            public static UInt32 LeftShift(UInt32 left, UInt32 right) { return (UInt32)(left << (int)right); }
            public static UInt64 LeftShift(UInt64 left, UInt64 right) { return (UInt64)(left << (int)right); }

            public static SByte RightShift(SByte left, SByte right) { return (SByte)(left >> right); }
            public static Int16 RightShift(Int16 left, Int16 right) { return (Int16)(left >> right); }
            public static Int32 RightShift(Int32 left, Int32 right) { return (Int32)(left >> right); }
            public static Int64 RightShift(Int64 left, Int64 right) { return (Int64)(left >> (int)right); }
            public static Byte RightShift(Byte left, Byte right) { return (Byte)(left >> right); }
            public static UInt16 RightShift(UInt16 left, UInt16 right) { return (UInt16)(left >> right); }
            public static UInt32 RightShift(UInt32 left, UInt32 right) { return (UInt32)(left >> (int)right); }
            public static UInt64 RightShift(UInt64 left, UInt64 right) { return (UInt64)(left >> (int)right); }

            public static Boolean And(Boolean left, Boolean right) { return left && right; }
            public static SByte And(SByte left, SByte right) { return (SByte)(left & right); }
            public static Int16 And(Int16 left, Int16 right) { return (Int16)(left & right); }
            public static Int32 And(Int32 left, Int32 right) { return (Int32)(left & right); }
            public static Int64 And(Int64 left, Int64 right) { return (Int64)(left & right); }
            public static Byte And(Byte left, Byte right) { return (Byte)(left & right); }
            public static UInt16 And(UInt16 left, UInt16 right) { return (UInt16)(left & right); }
            public static UInt32 And(UInt32 left, UInt32 right) { return (UInt32)(left & right); }
            public static UInt64 And(UInt64 left, UInt64 right) { return (UInt64)(left & right); }

            public static Boolean Or(Boolean left, Boolean right) { return left || right; }
            public static SByte Or(SByte left, SByte right) { return (SByte)(left | right); }
            public static Int16 Or(Int16 left, Int16 right) { return (Int16)(left | right); }
            public static Int32 Or(Int32 left, Int32 right) { return (Int32)(left | right); }
            public static Int64 Or(Int64 left, Int64 right) { return (Int64)(left | right); }
            public static Byte Or(Byte left, Byte right) { return (Byte)(left | right); }
            public static UInt16 Or(UInt16 left, UInt16 right) { return (UInt16)(left | right); }
            public static UInt32 Or(UInt32 left, UInt32 right) { return (UInt32)(left | right); }
            public static UInt64 Or(UInt64 left, UInt64 right) { return (UInt64)(left | right); }

            public static Boolean ExclusiveOr(Boolean left, Boolean right) { return left ^ right; }
            public static SByte ExclusiveOr(SByte left, SByte right) { return (SByte)(left ^ right); }
            public static Int16 ExclusiveOr(Int16 left, Int16 right) { return (Int16)(left ^ right); }
            public static Int32 ExclusiveOr(Int32 left, Int32 right) { return (Int32)(left ^ right); }
            public static Int64 ExclusiveOr(Int64 left, Int64 right) { return (Int64)(left ^ right); }
            public static Byte ExclusiveOr(Byte left, Byte right) { return (Byte)(left ^ right); }
            public static UInt16 ExclusiveOr(UInt16 left, UInt16 right) { return (UInt16)(left ^ right); }
            public static UInt32 ExclusiveOr(UInt32 left, UInt32 right) { return (UInt32)(left ^ right); }
            public static UInt64 ExclusiveOr(UInt64 left, UInt64 right) { return (UInt64)(left ^ right); }

            public static Boolean LessThan(SByte left, SByte right) { return left < right; }
            public static Boolean LessThan(Int16 left, Int16 right) { return left < right; }
            public static Boolean LessThan(Int32 left, Int32 right) { return left < right; }
            public static Boolean LessThan(Int64 left, Int64 right) { return left < right; }
            public static Boolean LessThan(Byte left, Byte right) { return left < right; }
            public static Boolean LessThan(UInt16 left, UInt16 right) { return left < right; }
            public static Boolean LessThan(UInt32 left, UInt32 right) { return left < right; }
            public static Boolean LessThan(UInt64 left, UInt64 right) { return left < right; }
            public static Boolean LessThan(Single left, Single right) { return left < right; }
            public static Boolean LessThan(Double left, Double right) { return left < right; }
            public static Boolean LessThan(Decimal left, Decimal right) { return left < right; }

            public static Boolean LessThanOrEqual(SByte left, SByte right) { return left <= right; }
            public static Boolean LessThanOrEqual(Int16 left, Int16 right) { return left <= right; }
            public static Boolean LessThanOrEqual(Int32 left, Int32 right) { return left <= right; }
            public static Boolean LessThanOrEqual(Int64 left, Int64 right) { return left <= right; }
            public static Boolean LessThanOrEqual(Byte left, Byte right) { return left <= right; }
            public static Boolean LessThanOrEqual(UInt16 left, UInt16 right) { return left <= right; }
            public static Boolean LessThanOrEqual(UInt32 left, UInt32 right) { return left <= right; }
            public static Boolean LessThanOrEqual(UInt64 left, UInt64 right) { return left <= right; }
            public static Boolean LessThanOrEqual(Single left, Single right) { return left <= right; }
            public static Boolean LessThanOrEqual(Double left, Double right) { return left <= right; }
            public static Boolean LessThanOrEqual(Decimal left, Decimal right) { return left <= right; }

            public static Boolean GreaterThan(SByte left, SByte right) { return left > right; }
            public static Boolean GreaterThan(Int16 left, Int16 right) { return left > right; }
            public static Boolean GreaterThan(Int32 left, Int32 right) { return left > right; }
            public static Boolean GreaterThan(Int64 left, Int64 right) { return left > right; }
            public static Boolean GreaterThan(Byte left, Byte right) { return left > right; }
            public static Boolean GreaterThan(UInt16 left, UInt16 right) { return left > right; }
            public static Boolean GreaterThan(UInt32 left, UInt32 right) { return left > right; }
            public static Boolean GreaterThan(UInt64 left, UInt64 right) { return left > right; }
            public static Boolean GreaterThan(Single left, Single right) { return left > right; }
            public static Boolean GreaterThan(Double left, Double right) { return left > right; }
            public static Boolean GreaterThan(Decimal left, Decimal right) { return left > right; }

            public static Boolean GreaterThanOrEqual(SByte left, SByte right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Int16 left, Int16 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Int32 left, Int32 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Int64 left, Int64 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Byte left, Byte right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(UInt16 left, UInt16 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(UInt32 left, UInt32 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(UInt64 left, UInt64 right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Single left, Single right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Double left, Double right) { return left >= right; }
            public static Boolean GreaterThanOrEqual(Decimal left, Decimal right) { return left >= right; }

            public static Boolean Equal(SByte left, SByte right) { return left == right; }
            public static Boolean Equal(Int16 left, Int16 right) { return left == right; }
            public static Boolean Equal(Int32 left, Int32 right) { return left == right; }
            public static Boolean Equal(Int64 left, Int64 right) { return left == right; }
            public static Boolean Equal(Byte left, Byte right) { return left == right; }
            public static Boolean Equal(UInt16 left, UInt16 right) { return left == right; }
            public static Boolean Equal(UInt32 left, UInt32 right) { return left == right; }
            public static Boolean Equal(UInt64 left, UInt64 right) { return left == right; }
            public static Boolean Equal(Single left, Single right) { return left == right; }
            public static Boolean Equal(Double left, Double right) { return left == right; }
            public static Boolean Equal(Decimal left, Decimal right) { return left == right; }
            public static Boolean Equal(object left, object right) { return left == right; }

            public static Boolean NotEqual(SByte left, SByte right) { return left != right; }
            public static Boolean NotEqual(Int16 left, Int16 right) { return left != right; }
            public static Boolean NotEqual(Int32 left, Int32 right) { return left != right; }
            public static Boolean NotEqual(Int64 left, Int64 right) { return left != right; }
            public static Boolean NotEqual(Byte left, Byte right) { return left != right; }
            public static Boolean NotEqual(UInt16 left, UInt16 right) { return left != right; }
            public static Boolean NotEqual(UInt32 left, UInt32 right) { return left != right; }
            public static Boolean NotEqual(UInt64 left, UInt64 right) { return left != right; }
            public static Boolean NotEqual(Single left, Single right) { return left != right; }
            public static Boolean NotEqual(Double left, Double right) { return left != right; }
            public static Boolean NotEqual(Decimal left, Decimal right) { return left != right; }
            public static Boolean NotEqual(object left, object right) { return left != right; }

            public static SByte Negate(SByte operand) { return (SByte)(-operand); }
            public static Int16 Negate(Int16 operand) { return (Int16)(-operand); }
            public static Int32 Negate(Int32 operand) { return (Int32)(-operand); }
            public static Int64 Negate(Int64 operand) { return (Int64)(-operand); }
            public static Byte Negate(Byte operand) { return (Byte)(-operand); }
            public static UInt16 Negate(UInt16 operand) { return (UInt16)(-operand); }
            public static UInt32 Negate(UInt32 operand) { return (UInt32)(-operand); }
            public static UInt64 Negate(UInt64 operand) { return (UInt64)(-(Int64)operand); }
            public static Single Negate(Single operand) { return (Single)(-operand); }
            public static Double Negate(Double operand) { return (Double)(-operand); }
            public static Decimal Negate(Decimal operand) { return (Decimal)(-operand); }
            public static SByte NegateChecked(SByte operand) { return checked((SByte)(-operand)); }
            public static Int16 NegateChecked(Int16 operand) { return checked((Int16)(-operand)); }
            public static Int32 NegateChecked(Int32 operand) { return checked((Int32)(-operand)); }
            public static Int64 NegateChecked(Int64 operand) { return checked((Int64)(-operand)); }
            public static Single NegateChecked(Single operand) { return checked((Single)(-operand)); }
            public static Double NegateChecked(Double operand) { return checked((Double)(-operand)); }
            public static Decimal NegateChecked(Decimal operand) { return checked((Decimal)(-operand)); }

            public static bool Not(bool operand) { return !operand; }
            public static SByte Not(SByte operand) { return (SByte)~operand; }
            public static Int16 Not(Int16 operand) { return (Int16)~operand; }
            public static Int32 Not(Int32 operand) { return (Int32)~operand; }
            public static Int64 Not(Int64 operand) { return (Int64)~operand; }
            public static Byte Not(Byte operand) { return (Byte)~operand; }
            public static UInt16 Not(UInt16 operand) { return (UInt16)~operand; }
            public static UInt32 Not(UInt32 operand) { return (UInt32)~operand; }
            public static UInt64 Not(UInt64 operand) { return (UInt64)~operand; }

            public static SByte ConvertToSByte(SByte operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Int16 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Int32 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Int64 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Byte operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(UInt16 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(UInt32 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(UInt64 operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Single operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Double operand) { return (SByte)operand; }
            public static SByte ConvertToSByte(Decimal operand) { return (SByte)operand; }
            public static SByte ConvertCheckedToSByte(SByte operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Int16 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Int32 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Int64 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Byte operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(UInt16 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(UInt32 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(UInt64 operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Single operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Double operand) { return checked((SByte)operand); }
            public static SByte ConvertCheckedToSByte(Decimal operand) { return checked((SByte)operand); }

            public static Int16 ConvertToInt16(SByte operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Int16 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Int32 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Int64 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Byte operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(UInt16 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(UInt32 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(UInt64 operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Single operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Double operand) { return (Int16)operand; }
            public static Int16 ConvertToInt16(Decimal operand) { return (Int16)operand; }
            public static Int16 ConvertCheckedToInt16(SByte operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Int16 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Int32 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Int64 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Byte operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(UInt16 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(UInt32 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(UInt64 operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Single operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Double operand) { return checked((Int16)operand); }
            public static Int16 ConvertCheckedToInt16(Decimal operand) { return checked((Int16)operand); }

            public static Int32 ConvertToInt32(SByte operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Int16 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Int32 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Int64 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Byte operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(UInt16 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(UInt32 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(UInt64 operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Single operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Double operand) { return (Int32)operand; }
            public static Int32 ConvertToInt32(Decimal operand) { return (Int32)operand; }
            public static Int32 ConvertCheckedToInt32(SByte operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Int16 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Int32 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Int64 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Byte operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(UInt16 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(UInt32 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(UInt64 operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Single operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Double operand) { return checked((Int32)operand); }
            public static Int32 ConvertCheckedToInt32(Decimal operand) { return checked((Int32)operand); }

            public static Int64 ConvertToInt64(SByte operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Int16 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Int32 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Int64 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Byte operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(UInt16 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(UInt32 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(UInt64 operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Single operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Double operand) { return (Int64)operand; }
            public static Int64 ConvertToInt64(Decimal operand) { return (Int64)operand; }
            public static Int64 ConvertCheckedToInt64(SByte operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Int16 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Int32 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Int64 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Byte operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(UInt16 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(UInt32 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(UInt64 operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Single operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Double operand) { return checked((Int64)operand); }
            public static Int64 ConvertCheckedToInt64(Decimal operand) { return checked((Int64)operand); }

            public static Byte ConvertToByte(SByte operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Int16 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Int32 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Int64 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Byte operand) { return (Byte)operand; }
            public static Byte ConvertToByte(UInt16 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(UInt32 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(UInt64 operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Single operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Double operand) { return (Byte)operand; }
            public static Byte ConvertToByte(Decimal operand) { return (Byte)operand; }
            public static Byte ConvertCheckedToByte(SByte operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Int16 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Int32 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Int64 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Byte operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(UInt16 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(UInt32 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(UInt64 operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Single operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Double operand) { return checked((Byte)operand); }
            public static Byte ConvertCheckedToByte(Decimal operand) { return checked((Byte)operand); }

            public static UInt16 ConvertToUInt16(SByte operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Int16 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Int32 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Int64 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Byte operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(UInt16 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(UInt32 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(UInt64 operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Single operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Double operand) { return (UInt16)operand; }
            public static UInt16 ConvertToUInt16(Decimal operand) { return (UInt16)operand; }
            public static UInt16 ConvertCheckedToUInt16(SByte operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Int16 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Int32 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Int64 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Byte operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(UInt16 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(UInt32 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(UInt64 operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Single operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Double operand) { return checked((UInt16)operand); }
            public static UInt16 ConvertCheckedToUInt16(Decimal operand) { return checked((UInt16)operand); }

            public static UInt32 ConvertToUInt32(SByte operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Int16 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Int32 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Int64 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Byte operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(UInt16 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(UInt32 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(UInt64 operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Single operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Double operand) { return (UInt32)operand; }
            public static UInt32 ConvertToUInt32(Decimal operand) { return (UInt32)operand; }
            public static UInt32 ConvertCheckedToUInt32(SByte operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Int16 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Int32 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Int64 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Byte operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(UInt16 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(UInt32 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(UInt64 operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Single operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Double operand) { return checked((UInt32)operand); }
            public static UInt32 ConvertCheckedToUInt32(Decimal operand) { return checked((UInt32)operand); }

            public static UInt64 ConvertToUInt64(SByte operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Int16 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Int32 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Int64 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Byte operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(UInt16 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(UInt32 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(UInt64 operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Single operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Double operand) { return (UInt64)operand; }
            public static UInt64 ConvertToUInt64(Decimal operand) { return (UInt64)operand; }
            public static UInt64 ConvertCheckedToUInt64(SByte operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Int16 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Int32 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Int64 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Byte operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(UInt16 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(UInt32 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(UInt64 operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Single operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Double operand) { return checked((UInt64)operand); }
            public static UInt64 ConvertCheckedToUInt64(Decimal operand) { return checked((UInt64)operand); }

            public static Single ConvertToSingle(SByte operand) { return (Single)operand; }
            public static Single ConvertToSingle(Int16 operand) { return (Single)operand; }
            public static Single ConvertToSingle(Int32 operand) { return (Single)operand; }
            public static Single ConvertToSingle(Int64 operand) { return (Single)operand; }
            public static Single ConvertToSingle(Byte operand) { return (Single)operand; }
            public static Single ConvertToSingle(UInt16 operand) { return (Single)operand; }
            public static Single ConvertToSingle(UInt32 operand) { return (Single)operand; }
            public static Single ConvertToSingle(UInt64 operand) { return (Single)operand; }
            public static Single ConvertToSingle(Single operand) { return (Single)operand; }
            public static Single ConvertToSingle(Double operand) { return (Single)operand; }
            public static Single ConvertToSingle(Decimal operand) { return (Single)operand; }
            public static Single ConvertCheckedToSingle(SByte operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Int16 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Int32 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Int64 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Byte operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(UInt16 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(UInt32 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(UInt64 operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Single operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Double operand) { return checked((Single)operand); }
            public static Single ConvertCheckedToSingle(Decimal operand) { return checked((Single)operand); }

            public static Double ConvertToDouble(SByte operand) { return (Double)operand; }
            public static Double ConvertToDouble(Int16 operand) { return (Double)operand; }
            public static Double ConvertToDouble(Int32 operand) { return (Double)operand; }
            public static Double ConvertToDouble(Int64 operand) { return (Double)operand; }
            public static Double ConvertToDouble(Byte operand) { return (Double)operand; }
            public static Double ConvertToDouble(UInt16 operand) { return (Double)operand; }
            public static Double ConvertToDouble(UInt32 operand) { return (Double)operand; }
            public static Double ConvertToDouble(UInt64 operand) { return (Double)operand; }
            public static Double ConvertToDouble(Single operand) { return (Double)operand; }
            public static Double ConvertToDouble(Double operand) { return (Double)operand; }
            public static Double ConvertToDouble(Decimal operand) { return (Double)operand; }
            public static Double ConvertCheckedToDouble(SByte operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Int16 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Int32 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Int64 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Byte operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(UInt16 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(UInt32 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(UInt64 operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Single operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Double operand) { return checked((Double)operand); }
            public static Double ConvertCheckedToDouble(Decimal operand) { return checked((Double)operand); }

            public static Decimal ConvertToDecimal(SByte operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Int16 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Int32 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Int64 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Byte operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(UInt16 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(UInt32 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(UInt64 operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Single operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Double operand) { return (Decimal)operand; }
            public static Decimal ConvertToDecimal(Decimal operand) { return (Decimal)operand; }
            public static Decimal ConvertCheckedToDecimal(SByte operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Int16 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Int32 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Int64 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Byte operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(UInt16 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(UInt32 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(UInt64 operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Single operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Double operand) { return checked((Decimal)operand); }
            public static Decimal ConvertCheckedToDecimal(Decimal operand) { return checked((Decimal)operand); }

            static Operators()
            {
                _methods = typeof(Operators).GetMethods(BindingFlags.Static | BindingFlags.Public).ToLookup(m => m.Name);
            }

            static ILookup<string, MethodInfo> _methods;

            public static IEnumerable<MethodInfo> GetOperatorMethods(string name)
            {
                return _methods[name];
            }
        }
    }
#endif
}