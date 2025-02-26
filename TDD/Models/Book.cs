namespace TDD.objects;

public class Book
{
    public required string Isbn { get; set; }
    public required string Titre { get; set; }
    public required string Auteur { get; set; }
    public required string Editeur { get; set; }
    public required BookFormat Format { get; set; }
    public bool Disponible { get; set; } = true;
}

public enum BookFormat
{
    Poche,
    Broché,
    GrandFormat
}