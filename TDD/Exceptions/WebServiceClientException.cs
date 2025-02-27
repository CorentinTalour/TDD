namespace TDD.Exceptions;

public class WebServiceDontFindBookByIsbn : Exception
{
    public WebServiceDontFindBookByIsbn()
        : base("Le web service n'a pas trouvé de livre avec cet ISBN.")
    {
    }
}