namespace Application.Interface;

public interface IStringFormatter
{
    string Format(string template, object target);
}