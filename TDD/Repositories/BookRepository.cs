using TDD.objects;

namespace TDD.Repositories;

public class BookRepository : IBookRepository
{
    private readonly TddDbContext _context;

    public BookRepository(TddDbContext context)
    {
        _context = context;
    }

    public Book Add(Book book)
    {
        _context.Books.Add(book);
        _context.SaveChanges();
        return book;
    }

    public Book Modify(Book book)
    {
        _context.Books.Update(book);
        _context.SaveChanges();
        return book;
    }

    public Book GetByIsbn(string isbn)
    {
        return _context.Books.FirstOrDefault(b => b.Isbn == isbn);
    }

    public IEnumerable<Book> GetAll()
    {
        return _context.Books.ToList();
    }

    public void Delete(string isbn)
    {
        var book = _context.Books.FirstOrDefault(b => b.Isbn == isbn);
        if (book != null)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }
    }

    public async Task<Book> Save(Book book)
    {
        _context.Books.Update(book);
        await _context.SaveChangesAsync();
        return book;
    }
}