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

    public bool CheckISBN(string isbn)
    {
        if (isbn.Contains('-') || isbn.Contains(' '))
            isbn = isbn.Replace("-", "").Replace(" ", "");

        if (isbn.Length != 10)
            throw new IsbnLengthException();

        int Sum = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                throw new IsbnFormatException();

            Sum += (isbn[i] - '0') * (i + 1);
        }

        char lastCharacter = isbn[9];
        int lastDigit;

        if (lastCharacter == 'X')
            lastDigit = 10;
        else if (char.IsDigit(lastCharacter))
            lastDigit = lastCharacter - '0';
        else
            throw new IsbnKeyException();

        Sum += lastDigit * 10;

        return Sum % 11 == 0;
    }

    public Book CreateBook(Book book)
    {
        if (string.IsNullOrWhiteSpace(book.Isbn) ||
            string.IsNullOrWhiteSpace(book.Title) ||
            string.IsNullOrWhiteSpace(book.Author) ||
            string.IsNullOrWhiteSpace(book.Publisher) ||
            !Enum.IsDefined(typeof(BookFormat), book.Format))
        {
            throw new BookArgumentException();
        }

        book.Available = true;
        return _bookRepository.Add(book);
    }

    public async Task<Book> ModifyBook(Book updatedBook)
    {
        Book existingBook = _bookRepository.GetByIsbn(updatedBook.Isbn);

        if (existingBook == null)
        {
            throw new BookNotFoundException();
        }

        existingBook.Title = updatedBook.Title;
        existingBook.Author = updatedBook.Author;
        existingBook.Publisher = updatedBook.Publisher;
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
            throw new BookNotFoundException();
        }
    }

    public IEnumerable<Book> SearchBooks(string isbn = null, string title = null, string author = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(isbn))
            {
                if (CheckISBN(isbn))
                {
                    Book book = _bookRepository.GetByIsbn(isbn);
                    if (book != null) return new List<Book> { book };
                    else return new List<Book>();
                }
            }

            if (!string.IsNullOrEmpty(title))
            {
                return _bookRepository.GetByTitle(title) ?? new List<Book>();
            }

            if (!string.IsNullOrEmpty(author))
            {
                return _bookRepository.GetByAuthor(author) ?? new List<Book>();
            }

            return new List<Book>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}