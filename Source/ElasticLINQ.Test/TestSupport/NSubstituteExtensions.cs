using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public static class NSubstituteExtensions
{
    public static T Arg<T>(this CallInfo callInfo, int index)
    {
        return (T)callInfo.Args()[index];
    }

    public static T Arg<T>(this ICall call)
    {
        return call.GetArguments().OfType<T>().SingleOrDefault();
    }

    public static T Arg<T>(this ICall call, int index)
    {
        return (T)call.GetArguments()[index];
    }

    public static CallInfo Captured<T>(this T substitute, Expression<Action<T>> expr)
    {
        return Captured<T>(substitute, 0, expr);
    }

    public static CallInfo Captured<T>(this T substitute, int callNumber, Expression<Action<T>> expr)
    {
        var call = GetMatchingCalls(substitute, expr)
            .ElementAtOrDefault(callNumber);

        if (call == null)
            throw new Exception("Cannot find matching call.");

        var methodParameters = call.GetParameterInfos();
        var arguments = new Argument[methodParameters.Length];
        var argumentValues = call.GetArguments();

        for (int i = 0; i < arguments.Length; i++)
        {
            var methodParameter = methodParameters[i];
            var argumentIndex = i;

            arguments[i] = new Argument(methodParameter.ParameterType, () => argumentValues[argumentIndex], _ => { });
        }

        return new CallInfo(arguments);
    }

    static MethodInfo ExtractMethodInfo<T>(Expression<Action<T>> expr)
    {
        if (expr.Body.NodeType == ExpressionType.Call)
        {
            return ((MethodCallExpression)expr.Body).Method;
        }

        throw new Exception("Cannot find method.");
    }

    static IEnumerable<ICall> GetMatchingCalls<T>(T substitute, Expression<Action<T>> expr)
    {
        var router = SubstitutionContext.Current.GetCallRouterFor(substitute);
        var method = ExtractMethodInfo(expr);
        var calls = router.ReceivedCalls();
        return calls.Where(c => c.GetMethodInfo() == method);
    }

    public static void Received<T>(this T instance, Action<T> action) where T : class
    {
        action(instance.Received());
    }

    public static void Received<T>(this T instance, int callCount, Action<T> action) where T : class
    {
        action(instance.Received(callCount));
    }

    public static void Received<T>(this T instance, Func<T, object> action) where T : class
    {
        action(instance.Received());
    }

    public static void Received<T>(this T instance, int callCount, Func<T, object> action) where T : class
    {
        action(instance.Received(callCount));
    }

    public static void ReturnsTask<T>(this Task<T> instance, Func<T> returnThis, params Func<T>[] returnThese) where T : class
    {
        Func<CallInfo, Task<T>>[] taskOfReturnThese =
            returnThese.Select<Func<T>, Func<CallInfo, Task<T>>>(x => _ => Task.FromResult<T>(x())).ToArray();

        instance.Returns(
            _ => Task.FromResult<T>(returnThis()),
            taskOfReturnThese);
    }

    public static void ReturnsTask<T>(this Task<T> instance, Func<CallInfo, T> valueFunc)
    {
        instance.Returns(callInfo => Task.FromResult<T>(valueFunc(callInfo)));
    }

    public static void ReturnsTask<T>(this Task<T> instance, T returnThis, params T[] returnThese)
    {
        instance.Returns(Task.FromResult<T>(returnThis), returnThese.Select(x => Task.FromResult<T>(x)).ToArray());
    }

    public static void ReturnsTaskForAnyArgs<T>(this Task<T> instance, T value, params T[] returnThese)
    {
        instance.ReturnsForAnyArgs(Task.FromResult<T>(value), returnThese.Select(x => Task.FromResult<T>(x)).ToArray());
    }

    public static void ReturnsTaskForAnyArgs<T>(this Task<T> instance, Func<T> valueFunc)
    {
        instance.ReturnsForAnyArgs(_ => Task.FromResult<T>(valueFunc()));
    }

    public static void ReturnsTaskForAnyArgs<T>(this Task<T> instance, Func<CallInfo, T> valueFunc)
    {
        instance.ReturnsForAnyArgs(callInfo => Task.FromResult<T>(valueFunc(callInfo)));
    }

    public static void Throws<T>(this T instance, Exception exception)
    {
        instance.Returns(_ => { throw exception; });
    }

    public static void Throws<T>(this T instance, Func<Exception> exceptionFunc)
    {
        instance.Returns(_ => { throw exceptionFunc(); });
    }

    public static void Throws<T>(this T instance, Func<CallInfo, Exception> exceptionFunc)
    {
        instance.Returns(callInfo => { throw exceptionFunc(callInfo); });
    }

    public static void Throws<T>(this T instance, Action<T> substitute, Exception exception) where T : class
    {
        instance.When(substitute).Do(_ => { throw exception; });
    }

    public static void ThrowsForAnyArgs<T>(this T instance, Exception exception)
    {
        instance.ReturnsForAnyArgs(_ => { throw exception; });
    }

    public static void ThrowsForAnyArgs<T>(this T instance, Func<Exception> exceptionFunc)
    {
        instance.ReturnsForAnyArgs(_ => { throw exceptionFunc(); });
    }

    public static void ThrowsForAnyArgs<T>(this T instance, Func<CallInfo, Exception> exceptionFunc)
    {
        instance.ReturnsForAnyArgs(callInfo => { throw exceptionFunc(callInfo); });
    }

    public static void ThrowsForAnyArgs<T>(this T instance, Action<T> substitute, Exception exception) where T : class
    {
        instance.WhenForAnyArgs(substitute).Do(_ => { throw exception; });
    }

    public static void ThrowsTask(this Task instance, Exception exception)
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.SetException(exception);
        instance.Returns(tcs.Task);
    }

    public static void ThrowsTask<T>(this Task<T> instance, Exception exception)
    {
        var tcs = new TaskCompletionSource<T>();
        tcs.SetException(exception);
        instance.Returns(tcs.Task);
    }

    public static void ThrowsTaskForAnyArgs(this Task instance, Exception exception)
    {
        var tcs = new TaskCompletionSource<object>();
        tcs.SetException(exception);
        instance.ReturnsForAnyArgs(tcs.Task);
    }

    public static void ThrowsTaskForAnyArgs<T>(this Task<T> instance, Exception exception)
    {
        var tcs = new TaskCompletionSource<T>();
        tcs.SetException(exception);
        instance.ReturnsForAnyArgs(tcs.Task);
    }

}