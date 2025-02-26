using TDD.objects;

namespace TDD.Repository;

public interface IBookRepository
{
    Book Add(Book book);
    Book GetByIsbn(string isbn);
    IEnumerable<Book> GetAll();
    void Delete(string isbn);
}