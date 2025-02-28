namespace TDD.Exceptions;

public class ReservationLimitExceededException : Exception
{
    public ReservationLimitExceededException()
        : base("Limite de 3 réservations ouvertes atteinte.")
    {
    }
}

public class MemberNotFoundException : Exception
{
    public MemberNotFoundException()
        : base("L'adhérent n'a pas été trouvé.")
    {
    }
}

public class InvalidReservationDateException : Exception
{
    public InvalidReservationDateException()
        : base("La date de réservation doit être comprise entre aujourd'hui et 4 mois.")
    {
    }
}