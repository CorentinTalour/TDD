using Moq;
using TDD.Exceptions;
using TDD.objects;
using TDD.Repository;
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
    [DataRow("2267046903", "Le seigneur des anneaux T3 Le retour du roi", "J.R.R. Tolkien", "BOURGOIS")]
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

    #endregion
}