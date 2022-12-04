using System.Collections.Concurrent;
using System.Linq.Expressions;
using Application.Exception;

namespace Application.Service;

public class CacheService
{
    private ConcurrentDictionary<string, Func<object, string>> _cacheStorage = new();

    public bool ContainsElement(string key)
    {
        return _cacheStorage.ContainsKey(key);
    }

    public Func<object, string> GetElement(string key)
    {
        return _cacheStorage[key];
    }

    public Func<object, string> AddElement(string key, string func, object template)
    {
        try
        {
            ParameterExpression parameterIdentifier = Expression.Parameter(typeof(object));
            MemberExpression propertyOrField = Expression.PropertyOrField(Expression.TypeAs(parameterIdentifier, template.GetType()), func);
            MethodCallExpression methodCallExpression = Expression.Call(propertyOrField, "ToString", null, null);
            Expression<Func<object, string>> expression = Expression.Lambda<Func<object, string>>(
                methodCallExpression,
                new ParameterExpression[] { parameterIdentifier }
            );
            Func<object, string> e = expression.Compile();
            _cacheStorage.TryAdd(key, e);
            return e;
                    
        }
        catch (System.Exception)
        {
            throw new PropertyOrFieldNotExistException("Field or property " + func + " not found in object " + template);
        }
    }
}