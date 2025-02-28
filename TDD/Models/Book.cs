using System.ComponentModel.DataAnnotations;

namespace TDD.Models;

public class Book
{
    [MaxLength(255)] public required string Isbn { get; set; }
    [MaxLength(255)] public required string Title { get; set; }
    [MaxLength(255)] public required string Author { get; set; }
    [MaxLength(255)] public required string Publisher { get; set; }
    public required BookFormat Format { get; set; }
    public bool Available { get; set; } = true;
}

public enum BookFormat
{
    Paperback,
    Hardcover,
    LargeFormat
}