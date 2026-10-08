using System;
using System.Linq;
using System.Reflection;

namespace Documentation;

public class Specifier<T> : ISpecifier
{
    private readonly Type _type;

    public Specifier()
    {
        _type = typeof(T);
    }

    public string GetApiDescription()
    {
        var attribute = _type.GetCustomAttribute<ApiDescriptionAttribute>();
        return attribute?.Description;
    }

    public string[] GetApiMethodNames()
    {
        return _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute<ApiMethodAttribute>() != null)
            .Select(m => m.Name)
            .ToArray();
    }

    public string GetApiMethodDescription(string methodName)
    {
        var method = GetMethod(methodName);
        if (method == null) return null;

        var attribute = method.GetCustomAttribute<ApiDescriptionAttribute>();
        return attribute?.Description;
    }

    public string[] GetApiMethodParamNames(string methodName)
    {
        var method = GetMethod(methodName);
        if (method == null) return null;

        return method.GetParameters()
            .Select(p => p.Name)
            .ToArray();
    }

    public string GetApiMethodParamDescription(string methodName, string paramName)
    {
        var method = GetMethod(methodName);
        if (method == null) return null;

        var parameter = method.GetParameters().FirstOrDefault(p => p.Name == paramName);
        if (parameter == null) return null;

        var attribute = parameter.GetCustomAttribute<ApiDescriptionAttribute>();
        return attribute?.Description;
    }

    public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName)
    {
        var method = GetMethod(methodName);
        if (method == null)
            return CreateEmptyParamDescription(paramName);

        var parameter = method.GetParameters().FirstOrDefault(p => p.Name == paramName);
        if (parameter == null)
            return CreateEmptyParamDescription(paramName);

        var result = CreateEmptyParamDescription(paramName);

        ApplyDescriptionAttribute(parameter, result);
        ApplyRequiredAttribute(parameter, result);
        ApplyIntValidationAttribute(parameter, result);

        return result;
    }

    public ApiMethodDescription GetApiMethodFullDescription(string methodName)
    {
        var method = GetMethod(methodName);
        if (method == null) return null;

        if (method.GetCustomAttribute<ApiMethodAttribute>() == null) return null;

        return new ApiMethodDescription
        {
            MethodDescription = new CommonDescription(methodName, GetApiMethodDescription(methodName)),
            ParamDescriptions = GetParamDescriptions(method),
            ReturnDescription = GetReturnDescription(method)
        };
    }

    private MethodInfo GetMethod(string methodName)
    {
        return _type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
    }

    private ApiParamDescription[] GetParamDescriptions(MethodInfo method)
    {
        return method.GetParameters()
            .Select(p => GetApiMethodParamFullDescription(method.Name, p.Name))
            .ToArray();
    }

    private ApiParamDescription CreateEmptyParamDescription(string paramName)
    {
        return new ApiParamDescription
        {
            ParamDescription = new CommonDescription(paramName),
            Required = false,
            MinValue = null,
            MaxValue = null
        };
    }

    private void ApplyDescriptionAttribute(ParameterInfo parameter, ApiParamDescription result)
    {
        var descriptionAttr = parameter.GetCustomAttribute<ApiDescriptionAttribute>();
        if (descriptionAttr != null)
            result.ParamDescription.Description = descriptionAttr.Description;
    }

    private void ApplyRequiredAttribute(ParameterInfo parameter, ApiParamDescription result)
    {
        var requiredAttr = parameter.GetCustomAttribute<ApiRequiredAttribute>();
        if (requiredAttr != null)
            result.Required = requiredAttr.Required;
    }

    private void ApplyIntValidationAttribute(ParameterInfo parameter, ApiParamDescription result)
    {
        var validationAttr = parameter.GetCustomAttribute<ApiIntValidationAttribute>();
        if (validationAttr != null)
        {
            result.MinValue = validationAttr.MinValue;
            result.MaxValue = validationAttr.MaxValue;
        }
    }

    private bool HasReturnAttributes(MethodInfo method)
    {
        if (method.ReturnType == typeof(void))
            return false;

        var requiredAttr = method.ReturnParameter.GetCustomAttribute<ApiRequiredAttribute>();
        var validationAttr = method.ReturnParameter.GetCustomAttribute<ApiIntValidationAttribute>();

        return requiredAttr != null || validationAttr != null;
    }

    private ApiParamDescription GetReturnDescription(MethodInfo method)
    {
        if (!HasReturnAttributes(method))
            return null;

        var result = new ApiParamDescription
        {
            ParamDescription = new CommonDescription(),
            Required = false,
            MinValue = null,
            MaxValue = null
        };

        var requiredAttr = method.ReturnParameter.GetCustomAttribute<ApiRequiredAttribute>();
        if (requiredAttr != null)
            result.Required = requiredAttr.Required;

        var validationAttr = method.ReturnParameter.GetCustomAttribute<ApiIntValidationAttribute>();
        if (validationAttr != null)
        {
            result.MinValue = validationAttr.MinValue;
            result.MaxValue = validationAttr.MaxValue;
        }

        return result;
    }
}