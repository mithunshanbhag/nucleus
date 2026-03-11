namespace Nucleus.Exceptions;

public abstract class NExceptionBase : Exception
{
    protected NExceptionBase(string message) : base(message)
    {
    }

    protected NExceptionBase(string message, Exception innerException) : base(message, innerException)
    {
    }

    public abstract IActionResult ToActionResult();

    public abstract HttpResponseMessage ToHttpResponseMessage();
}