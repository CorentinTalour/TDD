using System.Collections;
using Moq;
using TDD.Exceptions;
using TDD.objects;
using TDD.Repositories;
using TDD.services;

namespace TestTDD;

[TestClass]
public class BookTest
{
    private Mock<IBookRepository> _mockBookRepository;
    private BookService _bookService;

    private Mock<IBookWebService> _mockWebServiceClient;
    private BookWebService _bookWebService;

    [TestInitialize]
    public void Setup()
    {
        _mockBookRepository = new Mock<IBookRepository>();
        _bookService = new BookService(_mockBookRepository.Object);

        _mockWebServiceClient = new Mock<IBookWebService>();
        _bookWebService = new BookWebService(_mockBookRepository.Object, _mockWebServiceClient.Object);
    }


    #region Book

    [TestMethod]
    public void CreateBook_ShouldReturnBook()
    {
        Book book = new Book
        {
            Isbn = "2267046903",
            Title = "Le seigneur des anneaux T3 Le retour du roi",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.Paperback
        };

        _mockBookRepository.Setup(repo => repo.Add(It.IsAny<Book>())).Returns(book);

        Book result = _bookService.CreateBook(book);

        Assert.AreEqual(book, result);

        //Vérifie que Add a été appelé une fois
        _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()),
            Times.Once);
    }

    [DataTestMethod]
    [DataRow("", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "BOURGOIS", BookFormat.Paperback)]
    [DataRow("2267046903", "", "J.R.R. Tolkien", "BOURGOIS", BookFormat.Paperback)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "", "BOURGOIS", BookFormat.Paperback)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "", BookFormat.Paperback)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "BOURGOIS", 999)]
    public void CreateBook_WithEmptyInformation_ShouldThrowBookArgumentException(string isbn, string title,
        string author, string publisher, BookFormat format)
    {
        Book book = new Book
        {
            Isbn = isbn,
            Title = title,
            Author = author,
            Publisher = publisher,
            Format = format
        };

        BookArgumentException exception =
            Assert.ThrowsException<BookArgumentException>(() => _bookService.CreateBook(book));

        //Vérifie que la méthode Add n'a pas été appelée
        _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Never);
    }

    [TestMethod]
    public async Task CompleteBookWithWebService_BookExists_ShouldSaveBook()
    {
        string isbn = "2267046903";
        Book bookFromWebService = new Book
        {
            Isbn = isbn,
            Title = "Le seigneur des anneaux T3 Le retour du roi",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.LargeFormat,
        };

        _mockWebServiceClient.Setup(client => client.FindBookByIsbn(isbn))
            .Returns(Task.FromResult(bookFromWebService));

        _mockBookRepository.Setup(repo => repo.Save(bookFromWebService))
            .Returns(Task.FromResult(bookFromWebService));

        Book result = await _bookWebService.CompleteBookWithWebService(isbn);

        Assert.IsNotNull(result);
        Assert.AreEqual(isbn, result.Isbn);
        _mockWebServiceClient.Verify(client => client.FindBookByIsbn(isbn), Times.Once);
        _mockBookRepository.Verify(repo => repo.Save(bookFromWebService), Times.Once);
    }

    [TestMethod]
    public async Task CompleteBookWithWebService_BookNotFound_ShouldThrowException()
    {
        string isbn = "2267046903";

        _mockWebServiceClient.Setup(client => client.FindBookByIsbn(isbn))
            .ReturnsAsync((Book)null); //Retourne null pour simuler l'absence du livre

        await Assert.ThrowsExceptionAsync<WebServiceDontFindBookByIsbn>(async () =>
        {
            await _bookWebService.CompleteBookWithWebService(isbn);
        });

        _mockWebServiceClient.Verify(client => client.FindBookByIsbn(isbn), Times.Once);

        _mockBookRepository.Verify(repo => repo.Save(It.IsAny<Book>()), Times.Never);
    }

    [TestMethod]
    public async Task ModifyBook_BookExists_ShouldModifyAndSaveBook()
    {
        string isbn = "2267046903";
        Book existingBook = new Book
        {
            Isbn = isbn,
            Title = "Le seigneur des anneaux T3 Le retour du roi",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.Paperback
        };

        Book updatedBook = new Book
        {
            Isbn = isbn,
            Title = "Le seigneur des anneaux T3 La bataille pour la Terre du Milieu",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.LargeFormat
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(existingBook);

        _mockBookRepository.Setup(repo => repo.Save(existingBook)).Returns(Task.FromResult(existingBook));

        Book result = await _bookService.ModifyBook(updatedBook);

        Assert.IsNotNull(result);
        Assert.AreEqual(updatedBook.Title, result.Title);
        Assert.AreEqual(updatedBook.Author, result.Author);
        Assert.AreEqual(updatedBook.Publisher, result.Publisher);
        Assert.AreEqual(updatedBook.Format, result.Format);

        _mockBookRepository.Verify(repo => repo.GetByIsbn(isbn), Times.Once);

        _mockBookRepository.Verify(repo => repo.Save(existingBook), Times.Once);
    }

    // [TestMethod]
    // public async Task ModifierLivre_LivreExistePas_LeveUneException()
    // {
    //     string isbn = "2267046903";
    //     Book updatedBook = new Book
    //     {
    //         Isbn = isbn,
    //         Titre = "Le seigneur des anneaux T3 La bataille pour la Terre du Milieu",
    //         Auteur = "J.R.R. Tolkien",
    //         Editeur = "BOURGOIS",
    //         Format = BookFormat.GrandFormat
    //     };
    //
    //     Assert.ThrowsException<IsbnLengthException>(() => _bookService.ModifyBook(updatedBook));
    // }

    [TestMethod]
    public void DeleteBook_BookExists_ShouldDeleteBook()
    {
        string isbn = "2267046903";
        Book existingBook = new Book
        {
            Isbn = isbn,
            Title = "Le seigneur des anneaux T3 Le retour du roi",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.Hardcover
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(existingBook);

        _mockBookRepository.Setup(repo => repo.Delete(isbn)).Verifiable();

        _bookService.DeleteBook(isbn);

        _mockBookRepository.Verify(repo => repo.Delete(isbn), Times.Once);
    }

    [TestMethod]
    public void DeleteBook_BookDoesNotExist_ShouldThrowException()
    {
        string isbn = "2267046903";

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn))
            .Returns((Book)null); //Retourne null pour simuler l'absence du livre

        var exception = Assert.ThrowsException<BookNotFoundException>(() => _bookService.DeleteBook(isbn));

        Assert.IsInstanceOfType(exception, typeof(BookNotFoundException));

        _mockBookRepository.Verify(repo => repo.GetByIsbn(isbn), Times.Once);

        _mockBookRepository.Verify(repo => repo.Delete(isbn), Times.Never);
    }

    [TestMethod]
    public void SearchBooks_ByIsbn_ShouldReturnBook()
    {
        string isbn = "2267046903";
        Book expectedBook = new Book
        {
            Isbn = isbn,
            Title = "Le seigneur des anneaux",
            Author = "J.R.R. Tolkien",
            Publisher = "BOURGOIS",
            Format = BookFormat.Paperback
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(expectedBook);

        IEnumerable<Book> result = _bookService.SearchBooks(isbn: isbn);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count()); //1 livre retourné
        Assert.AreEqual(expectedBook.Isbn, result.First().Isbn);

        _mockBookRepository.Verify(repo => repo.GetByIsbn(isbn), Times.Once);
    }

    [TestMethod]
    public void SearchBooks_ByTitle_ShouldReturnBooks()
    {
        string title = "Le seigneur des anneaux";
        List<Book> expectedBooks = new List<Book>
        {
            new Book
            {
                Isbn = "2267046903",
                Title = title,
                Author = "J.R.R. Tolkien",
                Publisher = "BOURGOIS",
                Format = BookFormat.Paperback
            },
            new Book
            {
                Isbn = "2267046904",
                Title = title,
                Author = "J.R.R. Tolkien",
                Publisher = "BOURGOIS",
                Format = BookFormat.LargeFormat
            },
            new Book
            {
                Isbn = "2267046904",
                Title = "Bilbo le Hobbit",
                Author = "J.R.R. Tolkien",
                Publisher = "BOURGOIS",
                Format = BookFormat.LargeFormat
            }
        };

        _mockBookRepository.Setup(repo => repo.GetByTitle(title))
            .Returns(expectedBooks.Where(book => book.Title == title).ToList());

        IEnumerable<Book> result = _bookService.SearchBooks(title: title);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count()); //2 livres retournés
        Assert.AreEqual(expectedBooks.First().Title, result.First().Title);

        _mockBookRepository.Verify(repo => repo.GetByTitle(title), Times.Once);
    }

    [TestMethod]
    public void SearchBooks_ByAuthor_ShouldReturnBooks()
    {
        string author = "J.R.R. Tolkien";
        List<Book> expectedBooks = new List<Book>
        {
            new Book
            {
                Isbn = "2267046903",
                Title = "Le seigneur des anneaux",
                Author = author,
                Publisher = "BOURGOIS",
                Format = BookFormat.Paperback
            },
            new Book
            {
                Isbn = "2267046904",
                Title = "Bilbo le Hobbit",
                Author = author,
                Publisher = "BOURGOIS",
                Format = BookFormat.LargeFormat
            },
            new Book
            {
                Isbn = "2267046904",
                Title = "Bilbo le Hobbit",
                Author = "OwO",
                Publisher = "BOURGOIS",
                Format = BookFormat.LargeFormat
            }
        };

        _mockBookRepository.Setup(repo => repo.GetByAuthor(author))
            .Returns(expectedBooks.Where(book => book.Author == author).ToList());

        IEnumerable<Book> result = _bookService.SearchBooks(author: author);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count()); //2 livres retournés
        Assert.AreEqual(expectedBooks.First().Author, result.First().Author);

        _mockBookRepository.Verify(repo => repo.GetByAuthor(author), Times.Once);
    }

    #endregion
}

#region ISBN 10

[TestClass]
public class Isbn10Test
{
    private Mock<IBookRepository> _mockBookRepository;
    private BookService _bookService;

    [TestInitialize]
    public void Setup()
    {
        _mockBookRepository = new Mock<IBookRepository>();

        _bookService = new BookService(_mockBookRepository.Object);
    }

    [DataTestMethod]
    [DataRow("2970154706")]
    [DataRow("3455503950")]
    [DataRow("2012036961")]
    [DataRow("080442957X")]
    //Test avec tiret
    [DataRow("2-970154706")]
    [DataRow("345550395-0")]
    [DataRow("201-2036961")]
    [DataRow("0-8044-2957-X")]
    //Test avec espace
    [DataRow("2 970154706")]
    [DataRow("345550 3950")]
    [DataRow("2012 036961")]
    [DataRow("08 04429 57 X")]
    public void GivenValidIsbn10_WhenChecked_ShouldReturnTrue(string isbn)
    {
        bool result = _bookService.CheckISBN(isbn);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("2970154707")]
    [DataRow("3455503959")]
    [DataRow("2012036968")]
    [DataRow("201203696X")]
    public void GivenInvalidIsbn10_WhenChecked_ShouldReturnFalse(string isbn)
    {
        bool result = _bookService.CheckISBN(isbn);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("297015470")]
    [DataRow("29701547081")]
    public void GivenIsbn10WithIncorrectLength_WhenChecked_ShouldThrowIsbnLengthException(string isbn)
    {
        Assert.ThrowsException<IsbnLengthException>(() => _bookService.CheckISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297R154701")]
    [DataRow("29d015g706")]
    [DataRow("A2901570q6")]
    public void GivenIsbn10WithLetters_WhenChecked_ShouldThrowIsbnFormatException(string isbn)
    {
        Assert.ThrowsException<IsbnFormatException>(() => _bookService.CheckISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297015470R")]
    [DataRow("345550395j")]
    public void GivenIsbn10WithInvalidKeyLetter_WhenChecked_ShouldThrowIsbnKeyException(string isbn)
    {
        Assert.ThrowsException<IsbnKeyException>(() => _bookService.CheckISBN(isbn));
    }
}

#endregion