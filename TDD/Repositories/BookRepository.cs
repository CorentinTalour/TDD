using TDD.objects;

namespace TDD.Repository;

public class BookRepository : IBookRepository
{
    private readonly TddDbContext _context;

    public BookRepository(TddDbContext context)
    {
        _context = context;
    }

    // Ajouter un livre
    public Book Add(Book book)
    {
        _context.Books.Add(book); // Ajoute le livre à la base de données
        _context.SaveChanges(); // Sauvegarde les changements
        return book; // Retourne le livre ajouté
    }

    // Récupérer un livre par son ISBN
    public Book GetByIsbn(string isbn)
    {
        return _context.Books.FirstOrDefault(b => b.Isbn == isbn);
    }

    // Récupérer tous les livres
    public IEnumerable<Book> GetAll()
    {
        return _context.Books.ToList();
    }

    // Supprimer un livre par son ISBN
    public void Delete(string isbn)
    {
        var book = _context.Books.FirstOrDefault(b => b.Isbn == isbn);
        if (book != null)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }
    }
}