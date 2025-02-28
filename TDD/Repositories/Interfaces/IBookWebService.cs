using TDD.objects;

namespace TDD.Repositories.Interfaces;

public interface IBookWebService
{
    Task<Book> FindBookByIsbn(string isbn);
}