using TDD.Exceptions;
using TDD.services;

namespace TestTDD;

[TestClass]
public class BookTest
{
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
        var service = new BookService();

        var result = service.VerifierISBN(isbn);

        Assert.IsTrue(result);
    }

    [DataTestMethod]
    [DataRow("2970154707")]
    [DataRow("3455503959")]
    [DataRow("2012036968")]
    [DataRow("201203696X")]
    public void whenIsbn10IsNotValid_shouldReturnFalse(string isbn)
    {
        var service = new BookService();

        var result = service.VerifierISBN(isbn);

        Assert.IsFalse(result);
    }

    [DataTestMethod]
    [DataRow("297015470")]
    [DataRow("29701547081")]
    public void whenIsbn10NotContains10Digits_shouldReturnIsbnLengthException(string isbn)
    {
        var service = new BookService();

        Assert.ThrowsException<IsbnLengthException>(() => service.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297R154701")]
    [DataRow("29d015g706")]
    [DataRow("A2901570q6")]
    public void whenIsbn10ContainsLetter_shouldReturnIsbnFormatException(string isbn)
    {
        var service = new BookService();

        Assert.ThrowsException<IsbnFormatException>(() => service.VerifierISBN(isbn));
    }

    [DataTestMethod]
    [DataRow("297015470R")]
    [DataRow("345550395j")]
    public void whenIsbn10ContainsKeyLetter_shouldReturnIsbnKeyException(string isbn)
    {
        var service = new BookService();

        Assert.ThrowsException<IsbnKeyException>(() => service.VerifierISBN(isbn));
    }
}