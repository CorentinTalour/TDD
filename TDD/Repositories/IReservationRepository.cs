using TDD.objects;

namespace TDD.Repositories
{
    public interface IReservationRepository
    {
        List<Reservation> GetReservationsOuvertes();
        List<Reservation> GetReservationsDepassees(string adherentId);
        void Add(Reservation reservation);
        void Remove(Reservation reservation);
    }
}