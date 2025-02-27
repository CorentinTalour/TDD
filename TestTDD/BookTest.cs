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

    [TestInitialize]
    public void Setup()
    {
        _mockBookRepository = new Mock<IBookRepository>();

        _bookService = new BookService(_mockBookRepository.Object);
    }

    #region ISBN 10

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
    public void whenIsbn10IsValid_shouldReturnTrue(string isbn)
    {
        bool result = _bookService.VerifierISBN(isbn);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("2970154707")]
    [DataRow("3455503959")]
    [DataRow("2012036968")]
    [DataRow("201203696X")]
    public void whenIsbn10IsNotValid_shouldReturnFalse(string isbn)
    {
        bool result = _bookService.VerifierISBN(isbn);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("297015470")]
    [DataRow("29701547081")]
    public void whenIsbn10NotContains10Digits_shouldReturnIsbnLengthException(string isbn)
    {
        Assert.ThrowsException<IsbnLengthException>(() => _bookService.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297R154701")]
    [DataRow("29d015g706")]
    [DataRow("A2901570q6")]
    public void whenIsbn10ContainsLetter_shouldReturnIsbnFormatException(string isbn)
    {
        Assert.ThrowsException<IsbnFormatException>(() => _bookService.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297015470R")]
    [DataRow("345550395j")]
    public void whenIsbn10ContainsKeyLetter_shouldReturnIsbnKeyException(string isbn)
    {
        Assert.ThrowsException<IsbnKeyException>(() => _bookService.VerifierISBN(isbn));
    }

    #endregion

    #region Book

    [TestMethod]
    public void whenCreationBook_shouldReturnBook()
    {
        Book book = new Book
        {
            Isbn = "2267046903",
            Titre = "Le seigneur des anneaux T3 Le retour du roi",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.Poche
        };

        _mockBookRepository.Setup(repo => repo.Add(It.IsAny<Book>())).Returns(book);

        Book result = _bookService.CreateBook(book);

        Assert.AreEqual(book, result);

        //Vérifie que Add a été appelé une fois
        _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()),
            Times.Once);
    }

    [DataTestMethod]
    [DataRow("", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "BOURGOIS", BookFormat.Poche)]
    [DataRow("2267046903", "", "J.R.R. Tolkien", "BOURGOIS", BookFormat.Poche)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "", "BOURGOIS", BookFormat.Poche)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "", BookFormat.Poche)]
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "BOURGOIS", 999)]
    public void whenCreationBookWithEmptyInformation_shouldReturnBookArgumentException(string isbn, string titre,
        string auteur, string editeur, BookFormat format)
    {
        Book book = new Book
        {
            Isbn = isbn,
            Titre = titre,
            Auteur = auteur,
            Editeur = editeur,
            Format = format
        };

        BookArgumentException exception =
            Assert.ThrowsException<BookArgumentException>(() => _bookService.CreateBook(book));

        //Vérifie que la méthode Add n'a pas été appelée
        _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Never);
    }

    [TestMethod]
    public async Task CompleterLivreAvecWebService_LivreExiste_SauvegardeLivre()
    {
        Mock<IBookRepository> mockBookRepository;
        Mock<IBookWebService> mockWebServiceClient;
        BookWebService bookWebService;

        mockBookRepository = new Mock<IBookRepository>();
        mockWebServiceClient = new Mock<IBookWebService>();
        bookWebService = new BookWebService(mockBookRepository.Object, mockWebServiceClient.Object);

        string isbn = "2267046903";
        Book livreFromWebService = new Book
        {
            Isbn = isbn,
            Titre = "Le seigneur des anneaux T3 Le retour du roi",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.GrandFormat,
        };

        mockWebServiceClient.Setup(client => client.RechercherLivreParIsbn(isbn))
            .Returns(Task.FromResult(livreFromWebService));

        mockBookRepository.Setup(repo => repo.Save(livreFromWebService))
            .Returns(Task.FromResult(livreFromWebService));

        Book result = await bookWebService.CompleterLivreAvecWebService(isbn);

        Assert.IsNotNull(result);
        Assert.AreEqual(isbn, result.Isbn);
        mockWebServiceClient.Verify(client => client.RechercherLivreParIsbn(isbn), Times.Once);
        mockBookRepository.Verify(repo => repo.Save(livreFromWebService), Times.Once);
    }

    [TestMethod]
    public async Task CompleterLivreAvecWebService_LivreNonTrouve_LanceException()
    {
        Mock<IBookRepository> mockBookRepository = new Mock<IBookRepository>();
        Mock<IBookWebService> _mockWebServiceClient = new Mock<IBookWebService>();
        BookWebService bookWebService = new BookWebService(mockBookRepository.Object, _mockWebServiceClient.Object);

        string isbn = "2267046903";

        _mockWebServiceClient.Setup(client => client.RechercherLivreParIsbn(isbn))
            .ReturnsAsync((Book)null); //Retourne null pour simuler l'absence du livre

        await Assert.ThrowsExceptionAsync<WebServiceDontFindBookByIsbn>(async () =>
        {
            await bookWebService.CompleterLivreAvecWebService(isbn);
        });

        _mockWebServiceClient.Verify(client => client.RechercherLivreParIsbn(isbn), Times.Once);

        mockBookRepository.Verify(repo => repo.Save(It.IsAny<Book>()), Times.Never);
    }

    [TestMethod]
    public async Task ModifierLivre_LivreExiste_ModifieEtSauvegardeLivre()
    {
        string isbn = "2267046903";
        Book existingBook = new Book
        {
            Isbn = isbn,
            Titre = "Le seigneur des anneaux T3 Le retour du roi",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.Poche
        };

        Book updatedBook = new Book
        {
            Isbn = isbn,
            Titre = "Le seigneur des anneaux T3 La bataille pour la Terre du Milieu",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.GrandFormat
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(existingBook);

        _mockBookRepository.Setup(repo => repo.Save(existingBook)).Returns(Task.FromResult(existingBook));

        Book result = await _bookService.ModifyBook(updatedBook);

        Assert.IsNotNull(result);
        Assert.AreEqual(updatedBook.Titre, result.Titre);
        Assert.AreEqual(updatedBook.Auteur, result.Auteur);
        Assert.AreEqual(updatedBook.Editeur, result.Editeur);
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
    public void SupprimerLivre_LivreExiste_SupprimeLeLivre()
    {
        string isbn = "2267046903";
        Book existingBook = new Book
        {
            Isbn = isbn,
            Titre = "Le seigneur des anneaux T3 Le retour du roi",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.Broché
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(existingBook);

        _mockBookRepository.Setup(repo => repo.Delete(isbn)).Verifiable();

        _bookService.DeleteBook(isbn);

        _mockBookRepository.Verify(repo => repo.Delete(isbn), Times.Once);
    }

    [TestMethod]
    public void DeleteBook_LivreExistePas_LeveUneException()
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
    public void SearchBooks_ParIsbn_RetourneLivre()
    {
        string isbn = "2267046903";
        Book expectedBook = new Book
        {
            Isbn = isbn,
            Titre = "Le seigneur des anneaux",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.Poche
        };

        _mockBookRepository.Setup(repo => repo.GetByIsbn(isbn)).Returns(expectedBook);

        IEnumerable<Book> result = _bookService.SearchBooks(isbn: isbn);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count()); //1 livre retourné
        Assert.AreEqual(expectedBook.Isbn, result.First().Isbn);

        _mockBookRepository.Verify(repo => repo.GetByIsbn(isbn), Times.Once);
    }

    [TestMethod]
    public void SearchBooks_ParTitre_RetourneLivres()
    {
        string title = "Le seigneur des anneaux";
        List<Book> expectedBooks = new List<Book>
        {
            new Book
            {
                Isbn = "2267046903",
                Titre = title,
                Auteur = "J.R.R. Tolkien",
                Editeur = "BOURGOIS",
                Format = BookFormat.Poche
            },
            new Book
            {
                Isbn = "2267046904",
                Titre = title,
                Auteur = "J.R.R. Tolkien",
                Editeur = "BOURGOIS",
                Format = BookFormat.GrandFormat
            },
            new Book
            {
                Isbn = "2267046904",
                Titre = "Bilbo le Hobbit",
                Auteur = "J.R.R. Tolkien",
                Editeur = "BOURGOIS",
                Format = BookFormat.GrandFormat
            }
        };

        _mockBookRepository.Setup(repo => repo.GetByTitle(title))
            .Returns(expectedBooks.Where(book => book.Titre == title).ToList());

        IEnumerable<Book> result = _bookService.SearchBooks(title: title);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count()); //2 livres retournés
        Assert.AreEqual(expectedBooks.First().Titre, result.First().Titre);

        _mockBookRepository.Verify(repo => repo.GetByTitle(title), Times.Once);
    }

    [TestMethod]
    public void SearchBooks_ParAuteur_RetourneLivres()
    {
        string author = "J.R.R. Tolkien";
        List<Book> expectedBooks = new List<Book>
        {
            new Book
            {
                Isbn = "2267046903",
                Titre = "Le seigneur des anneaux",
                Auteur = author,
                Editeur = "BOURGOIS",
                Format = BookFormat.Poche
            },
            new Book
            {
                Isbn = "2267046904",
                Titre = "Bilbo le Hobbit",
                Auteur = author,
                Editeur = "BOURGOIS",
                Format = BookFormat.GrandFormat
            },
            new Book
            {
                Isbn = "2267046904",
                Titre = "Bilbo le Hobbit",
                Auteur = "OwO",
                Editeur = "BOURGOIS",
                Format = BookFormat.GrandFormat
            }
        };

        _mockBookRepository.Setup(repo => repo.GetByAuthor(author))
            .Returns(expectedBooks.Where(book => book.Auteur == author).ToList());

        IEnumerable<Book> result = _bookService.SearchBooks(author: author);

        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count()); //2 livres retournés
        Assert.AreEqual(expectedBooks.First().Auteur, result.First().Auteur);

        _mockBookRepository.Verify(repo => repo.GetByAuthor(author), Times.Once);
    }

    #endregion
}