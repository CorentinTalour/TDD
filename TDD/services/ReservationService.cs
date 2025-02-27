using TDD.Exceptions;
using TDD.Repositories;

namespace TDD.objects
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IAdherentRepository _adherentRepository;

        public ReservationService(IReservationRepository reservationRepository, IAdherentRepository adherentRepository)
        {
            _reservationRepository = reservationRepository;
            _adherentRepository = adherentRepository;
        }

        public string AjouterReservation(Adherent adherent, DateTime dateLimite)
        {
            if (adherent == null)
                throw new AdherentNotFoundException();

            // Vérifie que la date est dans la limite des 4 mois
            if (dateLimite < DateTime.Now || dateLimite > DateTime.Now.AddMonths(4))
                throw new InvalidReservationDateException();

            //Si l'adhérent a déjà 3 réservations ouvertes, on empêche l'ajout
            if (_adherentRepository.GetReservationsOuvertes(adherent.CodeAdherent).Count >= 3)
                throw new ReservationLimitExceededException();


            Reservation reservation = new Reservation(adherent, dateLimite);
            _reservationRepository.Add(reservation);

            return "Réservation ajoutée avec succès.";
        }

        public List<Reservation> GetReservationsOuvertes()
        {
            return _reservationRepository.GetReservationsOuvertes();
        }

        public void EnvoyerRappel(Adherent adherent)
        {
            List<Reservation> reservationsDepassees =
                _adherentRepository.GetReservationsDepassees(adherent.CodeAdherent);

            if (adherent == null)
                throw new AdherentNotFoundException();

            if (reservationsDepassees.Any())
            {
                //Simule l'envoi d'un email (ici simplement un log console pour le test)
                Console.WriteLine(
                    $"Envoi d'un rappel pour les réservations suivantes : {string.Join(", ",
                        reservationsDepassees.Select(r => r.CodeReservation))}");
            }
        }
    }
}