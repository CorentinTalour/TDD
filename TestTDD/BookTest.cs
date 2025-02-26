using TDD.Exceptions;
using TDD.services;

namespace TestTDD;

[TestClass]
public class BookTest
{
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
        BookService service = new BookService();

        bool result = service.VerifierISBN(isbn);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("2970154707")]
    [DataRow("3455503959")]
    [DataRow("2012036968")]
    [DataRow("201203696X")]
    public void whenIsbn10IsNotValid_shouldReturnFalse(string isbn)
    {
        BookService service = new BookService();

        bool result = service.VerifierISBN(isbn);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("297015470")]
    [DataRow("29701547081")]
    public void whenIsbn10NotContains10Digits_shouldReturnIsbnLengthException(string isbn)
    {
        BookService service = new BookService();

        Assert.ThrowsException<IsbnLengthException>(() => service.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297R154701")]
    [DataRow("29d015g706")]
    [DataRow("A2901570q6")]
    public void whenIsbn10ContainsLetter_shouldReturnIsbnFormatException(string isbn)
    {
        BookService service = new BookService();

        Assert.ThrowsException<IsbnFormatException>(() => service.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297015470R")]
    [DataRow("345550395j")]
    public void whenIsbn10ContainsKeyLetter_shouldReturnIsbnKeyException(string isbn)
    {
        BookService service = new BookService();

        Assert.ThrowsException<IsbnKeyException>(() => service.VerifierISBN(isbn));
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

    [TestMethod]
    public void whenCreationBookWithEmptyIsbn_shouldReturnBookArgumentException()
    {
        Book book = new Book
        {
            Isbn = "",
            Titre = "Le seigneur des anneaux T3 Le retour du roi",
            Auteur = "J.R.R. Tolkien",
            Editeur = "BOURGOIS",
            Format = BookFormat.Poche
        };

        BookArgumentException exception =
            Assert.ThrowsException<BookArgumentException>(() => _bookService.CreateBook(book));

        // Vérifie que la méthode Add n'a pas été appelée car une exception a été levée
        _mockBookRepository.Verify(repo => repo.Add(It.IsAny<Book>()), Times.Never);
    }

    #endregion
}