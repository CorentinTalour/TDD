using TDD.objects;

namespace TDD.Repositories;

public interface IBookRepository
{
    Book Add(Book book);
    Book Modify(Book book);
    Task<Book> Save(Book book);
    Book GetByIsbn(string isbn);
    IEnumerable<Book> GetAll();
    void Delete(string isbn);
}