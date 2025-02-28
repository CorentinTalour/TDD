using TDD.objects;

namespace TDD.Repositories;

public interface IBookWebService
{
    Task<Book> FindBookByIsbn(string isbn);
}