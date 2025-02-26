namespace TDD.services;

public class BookService
{
    public bool VerifierISBN(string isbn)
    {
        if (isbn.Contains('-') || isbn.Contains(' ')) 
            isbn = isbn.Replace("-", "").Replace(" ", "");

        if (isbn.Length != 10)
            return false;

        int somme = 0;
        for (int i = 0; i < 9; i++)
        {
            if (!char.IsDigit(isbn[i]))
                return false;
            
            somme += (isbn[i] - '0') * (i + 1);
        }

        char dernierCaractere = isbn[9];
        int dernierChiffre;
        
        if (dernierCaractere == 'X')
            dernierChiffre = 10;
        else if (char.IsDigit(dernierCaractere))
            dernierChiffre = dernierCaractere - '0';
        else
            return false;

        somme += dernierChiffre * 10;

        return (somme % 11 == 0);
    }
}