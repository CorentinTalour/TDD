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
            // Si l'adhérent a déjà 3 réservations ouvertes, on empêche l'ajout
            if (_adherentRepository.GetReservationsOuvertes(adherent.CodeAdherent).Count >= 3)
            {
                return "Limite de 3 réservations ouvertes atteinte.";
            }

            // Créer une nouvelle réservation
            var reservation = new Reservation(adherent, dateLimite);
            _reservationRepository.Add(reservation);

            return "Réservation ajoutée avec succès.";
        }

        public List<Reservation> GetReservationsOuvertes()
        {
            // Retourne toutes les réservations non-clôturées
            return _reservationRepository.GetReservationsOuvertes();
        }

        public void EnvoyerRappel(Adherent adherent)
        {
            var reservationsDepassees = _adherentRepository.GetReservationsDepassees(adherent.CodeAdherent);

            if (reservationsDepassees.Any())
            {
                // Simule l'envoi d'un email (ici simplement un log console pour le test)
                Console.WriteLine(
                    $"Envoi d'un rappel pour les réservations suivantes : {string.Join(", ", reservationsDepassees.Select(r => r.CodeReservation))}");
            }
        }
    }
}