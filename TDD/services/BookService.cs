using TDD.Exceptions;
using TDD.objects;
using TDD.Repositories;

namespace TDD.services;

public class BookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public bool VerifierISBN(string isbn)
    {
        if (isbn.Contains('-') || isbn.Contains(' '))
            isbn = isbn.Replace("-", "").Replace(" ", "");

        if (isbn.Length != 10)
            throw new IsbnLengthException();

        int somme = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                throw new IsbnFormatException();

            somme += (isbn[i] - '0') * (i + 1);
        }

        char dernierCaractere = isbn[9];
        int dernierChiffre;

        if (dernierCaractere == 'X')
            dernierChiffre = 10;
        else if (char.IsDigit(dernierCaractere))
            dernierChiffre = dernierCaractere - '0';
        else
            throw new IsbnKeyException();

        somme += dernierChiffre * 10;

        return somme % 11 == 0;
    }

    public Book CreateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Isbn) ||
            string.IsNullOrWhiteSpace(book.Titre) ||
            string.IsNullOrWhiteSpace(book.Auteur) ||
            string.IsNullOrWhiteSpace(book.Editeur) ||
            !Enum.IsDefined(typeof(BookFormat), book.Format))
        {
            throw new BookArgumentException();
        }

        book.Disponible = true;
        return _bookRepository.Add(book);
    }

    public async Task<Book> ModifyBook(Book updatedBook)
    {
        Book existingBook = _bookRepository.GetByIsbn(updatedBook.Isbn);

        if (existingBook == null)
        {
            throw new BookNotFoundException();
        }

        existingBook.Titre = updatedBook.Titre;
        existingBook.Auteur = updatedBook.Auteur;
        existingBook.Editeur = updatedBook.Editeur;
        existingBook.Format = updatedBook.Format;

        return await _bookRepository.Save(existingBook);
    }

    public void DeleteBook(string isbn)
    {
        Book? book = _bookRepository.GetByIsbn(isbn);

        if (book != null)
        {
            _bookRepository.Delete(isbn);
        }
        else
        {
            throw new Exception("");
        }
    }
}