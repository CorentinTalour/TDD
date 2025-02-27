using TDD.objects;

namespace TDD.Repositories;

public interface IAdherentRepository
{
    Adherent GetById(int adherentId);
    List<Reservation> GetReservationsOuvertes(string adherentId);
    List<Reservation> GetReservationsDepassees(string adherentId);
}