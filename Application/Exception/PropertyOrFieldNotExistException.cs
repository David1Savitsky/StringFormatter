namespace Application.Exception;

public class PropertyOrFieldNotExistException : System.Exception
{
    public PropertyOrFieldNotExistException(string message) : base(message)
    {
    }
}