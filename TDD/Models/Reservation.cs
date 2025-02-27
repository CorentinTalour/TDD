using System;

namespace TDD.objects
{
    public class Reservation
    {
        public string CodeReservation { get; set; }
        public Adherent Adherent { get; set; }
        public DateTime DateReservation { get; set; }
        public DateTime DateLimite { get; set; }
        public bool EstCloturee { get; set; }

        public Reservation(Adherent adherent, DateTime dateLimite)
        {
            Adherent = adherent;
            DateReservation = DateTime.Now;
            DateLimite = dateLimite;
            EstCloturee = false;
        }
    }
}