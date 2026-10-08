using System.Linq.Expressions;

namespace Reflection.Differentiation;

public static class Algebra
{
    public static Expression<Func<double, double>> Differentiate(
        Expression<Func<double, double>> function)
    {
        var parameter = function.Parameters[0];
        var body = DifferentiateExpression(function.Body, parameter);
        return Expression.Lambda<Func<double, double>>(body, parameter);
    }

    private static Expression DifferentiateExpression(Expression expr, ParameterExpression param)
    {
        switch (expr)
        {
            case ConstantExpression:
                return Expression.Constant(0.0);

            case ParameterExpression p when p == param:
                return Expression.Constant(1.0);

            case BinaryExpression binary:
                return DifferentiateBinary(binary, param);

            case MethodCallExpression call:
                return DifferentiateMethodCall(call, param);

            default:
                throw new ArgumentException($"Unsupported expression: {expr}");
        }
    }

    private static Expression DifferentiateBinary(BinaryExpression binary, ParameterExpression param)
    {
        var left = binary.Left;
        var right = binary.Right;
        var dLeft = DifferentiateExpression(left, param);
        var dRight = DifferentiateExpression(right, param);

        switch (binary.NodeType)
        {
            case ExpressionType.Add:
                return Expression.Add(dLeft, dRight);

            case ExpressionType.Subtract:
                return Expression.Subtract(dLeft, dRight);

            case ExpressionType.Multiply:
                return Expression.Add(
                    Expression.Multiply(dLeft, right),
                    Expression.Multiply(left, dRight));

            case ExpressionType.Divide:
                return Expression.Divide(
                    Expression.Subtract(
                        Expression.Multiply(dLeft, right),
                        Expression.Multiply(left, dRight)),
                    Expression.Multiply(right, right));

            default:
                throw new ArgumentException($"Unsupported operator: {binary.NodeType}");
        }
    }

    private static Expression DifferentiateMethodCall(MethodCallExpression call, ParameterExpression param)
    {
        if (call.Method.DeclaringType != typeof(Math))
            throw new ArgumentException($"Unsupported method: {call.Method.Name}");

        var argument = call.Arguments[0];
        var dArgument = DifferentiateExpression(argument, param);

        switch (call.Method.Name)
        {
            case nameof(Math.Sin):
                return Expression.Multiply(
                    Expression.Call(typeof(Math), nameof(Math.Cos), null, argument),
                    dArgument);

            case nameof(Math.Cos):
                return Expression.Multiply(
                    Expression.Negate(
                        Expression.Call(typeof(Math), nameof(Math.Sin), null, argument)),
                    dArgument);

            default:
                throw new ArgumentException($"Unsupported method: {call.Method.Name}");
        }
    }
}