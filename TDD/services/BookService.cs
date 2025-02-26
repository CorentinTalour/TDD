using TDD.Exceptions;

namespace TDD.services;

public class BookService
{
    public bool VerifierISBN(string isbn)
    {
        if (isbn.Contains('-') || isbn.Contains(' '))
            isbn = isbn.Replace("-", "").Replace(" ", "");

        if (isbn.Length != 10)
            throw new IsbnLengthException();

        var somme = 0;
        for (var i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                throw new IsbnFormatException();

            somme += (isbn[i] - '0') * (i + 1);
        }

        var dernierCaractere = isbn[9];
        int dernierChiffre;

        if (dernierCaractere == 'X')
            dernierChiffre = 10;
        else if (char.IsDigit(dernierCaractere))
            dernierChiffre = dernierCaractere - '0';
        else
            throw new IsbnKeyException();

        somme += dernierChiffre * 10;

        return somme % 11 == 0;
    }
}