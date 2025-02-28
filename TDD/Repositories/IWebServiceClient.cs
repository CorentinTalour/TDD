using TDD.objects;

namespace TDD.Repositories;

public interface IWebServiceClient
{
    Book SearchBookByIsbn(string isbn);
}