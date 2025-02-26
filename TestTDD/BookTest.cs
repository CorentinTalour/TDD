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
}