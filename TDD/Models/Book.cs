namespace TDD.objects;

public class Book
{
    public required string Isbn { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Publisher { get; set; }
    public required BookFormat Format { get; set; }
    public bool Available { get; set; } = true;
}

public enum BookFormat
{
    Paperback,
    Hardcover,
    LargeFormat
}