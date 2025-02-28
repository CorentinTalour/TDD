using TDD.Exceptions;
using TDD.Models;
using TDD.Repositories.Interfaces;

namespace TDD.services
{
    public class ReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMemberRepository _memberRepository;

        public ReservationService(IReservationRepository reservationRepository, IMemberRepository memberRepository)
        {
            _reservationRepository = reservationRepository;
            _memberRepository = memberRepository;
        }

        public string AddReservation(Member member, DateTime limitDate)
        {
            if (member == null)
                throw new MemberNotFoundException();

            if (limitDate < DateTime.Now || limitDate > DateTime.Now.AddMonths(4))
                throw new InvalidReservationDateException();

            //Si l'adhérent a déjà 3 réservations ouvertes, on empêche l'ajout
            if (_memberRepository.GetReservationsOuvertes(member.MemberCode).Count >= 3)
                throw new ReservationLimitExceededException();


            Reservation reservation = new Reservation(member, limitDate);
            _reservationRepository.Add(reservation);

            return "Réservation ajoutée avec succès.";
        }

        public List<Reservation> GetOpenReservations()
        {
            return _reservationRepository.GetReservationsOuvertes();
        }

        public void SendReminder(Member member)
        {
            List<Reservation> overdueReservations =
                _memberRepository.GetReservationsDepassees(member.MemberCode);

            if (member == null)
                throw new MemberNotFoundException();

            if (overdueReservations.Any())
            {
                //Simule l'envoi d'un email (ici simplement un log console pour le test)
                Console.WriteLine(
                    $"Envoi d'un rappel pour les réservations suivantes : {string.Join(", ",
                        overdueReservations.Select(r => r.ReservationCode))}");
            }
        }
    }
}