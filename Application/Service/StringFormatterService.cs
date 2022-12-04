using Application.Exception;
using Application.Interface;

namespace Application.Service;

public class StringFormatterService : IStringFormatter
{
    public static readonly StringFormatterService Shared = new();

    private int _validationCheck = 0;
    private readonly CacheService _cacheService = new();
    
    public string Format(string template, object target)
    {
        string formattedString = "";
        string fieldOrProperty = "";
        bool textFlag = false;
        bool screeningFlag = false;

        int templateLength = template.Length;
        for (int i = 0; i < templateLength; i++)
        {
            if (!Validate(template, i))
            {
                throw new InvalidInputException($"Symbol on {i} position is unavailable");
            }
            
            if (template[i] == '{')
            {
                if (textFlag)
                {
                    formattedString += template[i];
                    textFlag = !textFlag;
                    screeningFlag = true;
                }
                else
                {
                    fieldOrProperty = "";
                    textFlag = !textFlag;
                }
            }
                
            if (template[i] == '}')
            {
                if (screeningFlag)
                {
                    formattedString += template[i];
                    screeningFlag = !screeningFlag;
                }
                if (textFlag)
                {
                    formattedString += InvokeSubstitution(fieldOrProperty, target);
                    textFlag = !textFlag;
                }
            }

            if (template[i] == '}' || template[i] == '{') continue;
            if (!textFlag)
            {
                formattedString += template[i];
            }
            fieldOrProperty += template[i];
        }
        return formattedString;
    }

    private bool Validate(string template, int index)
    {
        if (_validationCheck is > 2 or < 0 ||
            (_validationCheck == 1 && template[index] == '{' && template[index - 1] != '{') ||
            (_validationCheck == 2 && template[index] == '}' && (index + 1 >= template.Length || template[index + 1] != '}'))
           )
        {
            return false;
        }
        if (template[index] == '{') _validationCheck++;
        if (template[index] == '}') _validationCheck--;

        if (template.Length == index + 1 && _validationCheck != 0) return false;
        
        return true;
    }
    
    private string InvokeSubstitution(string input, object target)
    {
        string key = target.GetType() + "." + input;

        if (_cacheService.ContainsElement(key))
        {
            Func<object, string> func = _cacheService.GetElement(key);
            return func(target);
        }

        Func<object, string> addedFunc = _cacheService.AddElement(key, input, target);
        return addedFunc(target);
    }
}