using TDD.objects;

namespace TDD.Repositories;

public interface IWebServiceClient
{
    Book RechercherLivreParIsbn(string isbn);
}