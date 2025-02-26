namespace TDD.Exceptions;

public class IsbnLengthException : Exception
{
    public IsbnLengthException()
        : base("L'ISBN doit contenir exactement 10 caractères.")
    {
    }
}

public class IsbnFormatException : Exception
{
    public IsbnFormatException()
        : base("L'ISBN ne doit contenir que des chiffres ou un X pour la clé.")
    {
    }
}

public class IsbnKeyException : Exception
{
    public IsbnKeyException()
        : base("La clé de l'ISBN doit être un chiffre ou un X.")
    {
    }
}

public class BookArgumentException : Exception
{
    public BookArgumentException()
        : base("Tous les champs sont obligatoires.")
    {
    }
}