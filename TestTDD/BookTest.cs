using TDD.Exceptions;
using TDD.services;

namespace TestTDD;

[TestClass]
public class BookTest
{
    [TestMethod]
    public void whenIsbn10IsValid_shouldReturnTrue()
    {
        BookService service = new BookService();
        string isbn = "2970154706";
        
        bool result = service.VerifierISBN(isbn);
        
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void whenIsbn10IsNotValid_shouldReturnFalse()
    {
        BookService service = new BookService();
        string isbn = "2970154701";
        
        bool result = service.VerifierISBN(isbn);
        
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void whenIsbn10NotContains10Digits_shouldReturnIsbnLengthException()
    {
        BookService service = new BookService();
        string isbn = "297015406";
        
        Assert.ThrowsException<IsbnLengthException>(() => service.VerifierISBN(isbn));
    }

    [TestMethod]
    public void whenIsbn10ContainsLetter_shouldReturnIsbnFormatException()
    {
        BookService service = new BookService();
        string isbn = "297015h706";
        
        Assert.ThrowsException<IsbnFormatException>(() => service.VerifierISBN(isbn));
    }

    [TestMethod]
    public void whenIsbn10ContainsKeyLetter_shouldReturnIsbnKeyException()
    {
        BookService service = new BookService();
        string isbn = "297015470h";
        
        Assert.ThrowsException<IsbnKeyException>(() => service.VerifierISBN(isbn));
    }

}