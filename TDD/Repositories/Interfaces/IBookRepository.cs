using TDD.Models;

namespace TDD.Repositories.Interfaces;

public interface IBookRepository
{
    Book Add(Book book);
    Book Modify(Book book);
    Task<Book> Save(Book book);
    Book? GetByIsbn(string isbn);
    IEnumerable<Book> GetByTitle(string title);
    IEnumerable<Book> GetByAuthor(string author);
    IEnumerable<Book> GetAll();
    void Delete(string isbn);
}