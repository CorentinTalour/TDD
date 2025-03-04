using TDD.Models;

namespace TDD.Repositories.Interfaces;

public interface IMemberRepository
{
    Member GetById(int adherentId);
    List<Reservation> GetReservationsOuvertes(string adherentId);
    List<Reservation> GetReservationsDepassees(string adherentId);
}